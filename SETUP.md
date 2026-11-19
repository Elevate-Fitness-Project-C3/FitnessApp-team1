# ID-02 – Setup

## 1) Packages (IdentityService)
```bash
dotnet add IdentityService package MailKit
dotnet add IdentityService package Microsoft.EntityFrameworkCore.SqlServer
dotnet add IdentityService package Microsoft.EntityFrameworkCore.Design
```
Source uses primary constructors / collection expressions -> .NET 8+ (C# 12).
Namespaces assume the project root namespace is `IdentityService`. Adjust if different.

## 2) Program.cs
```csharp
builder.Services.AddEmailVerification(builder.Configuration);
// ...
var app = builder.Build();

app.MapEmailVerificationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevFakeRegistration(); // fake registration until ID-01 is wired
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<EmailVerificationDbContext>().Database.Migrate();
}
```

## 3) Migration (generate it, don't hand-write it)
```bash
dotnet ef migrations add AddEmailVerificationChallenges \
  -c EmailVerificationDbContext \
  -o Infrastructure/Persistence/Migrations \
  -p IdentityService
```
It creates table `EmailVerificationChallenges` with indexes on UserId, ExpiresAt and a
filtered unique index (one active challenge per user). Its history table is separate
(`__EFMigrationsHistory_Verification`) so it can't clash with your other DbContext.

## 4) appsettings.json (non-secret)
```json
{
  "Verification": {
    "CodeLength": 6,
    "ExpiryMinutes": 10,
    "MaxAttempts": 5,
    "ResendCooldownSeconds": 60
  },
  "Email": {
    "From": "no-reply@local.test",
    "Smtp": { "Host": "localhost", "Port": 1025 }
  }
}
```
`Verification:HmacKey` (min 32 chars) is a secret: env var / user-secrets only.
```bash
openssl rand -base64 48
dotnet user-secrets set "Verification:HmacKey" "<value>"   # when running outside docker
```

## 5) Docker
`.env` (must be git-ignored):
```
SQL_SA_PASSWORD=...
VERIFICATION_HMAC_KEY=<output of openssl rand -base64 48>
```
`docker-compose.yml` (base) - add to identityservice:
```yaml
  identityservice:
    environment:
      - Verification__HmacKey=${VERIFICATION_HMAC_KEY:?VERIFICATION_HMAC_KEY is required}
```
`docker-compose.override.yml` (dev) - add:
```yaml
  identityservice:
    environment:
      - Email__From=no-reply@local.test
      - Email__Smtp__Host=mailhog
      - Email__Smtp__Port=1025
    depends_on:
      mailhog:
        condition: service_started

  mailhog:
    image: mailhog/mailhog
    ports:
      - "127.0.0.1:8025:8025"
```
(The override's `identityservice` block already exists; merge these keys into it.)

## 6) Try it
```bash
curl -X POST localhost:5001/api/dev/fake-register -H "Content-Type: application/json" -d '{"email":"test@example.com"}'
# open http://localhost:8025, read the code
curl -X POST localhost:5001/api/auth/verify-email -H "Content-Type: application/json" -d '{"email":"test@example.com","code":"123456"}'
curl -X POST localhost:5001/api/auth/resend-verification -H "Content-Type: application/json" -d '{"email":"test@example.com"}'
```
Note: the fake user store is in memory, so restarting the container forgets users.

## 7) Wiring to registration later
1. Implement `IUserEmailStore` on top of your real users table (GetByEmailAsync, MarkEmailConfirmedAsync) and register it instead of `InMemoryUserEmailStore`.
2. In the registration handler, after the user is saved: `await verification.IssueAsync(user.Id, user.Email, ct);`
3. In login (ID-0x), reject users whose email is not confirmed.
4. Delete `MapDevFakeRegistration`.

## Error responses (ProblemDetails, extension `code`)
| code | HTTP |
|---|---|
| code_invalid | 400 |
| code_expired | 400 |
| code_already_used | 409 |
| code_locked | 429 |

## Assumptions
- UserId is a Guid. Emails are normalized with trim + lowercase.
- Resend is silent (always 202 with the same message), including during cooldown, to avoid user enumeration.
- Sending failure doesn't fail registration; it's logged (without the code) and the user can resend.
- `EmailVerificationService` uses the DbContext directly because atomic `ExecuteUpdate` is what makes "use once" safe.
- Not included: per-IP rate limiting (add ASP.NET Core `AddRateLimiter` on `/api/auth/*`), and a cleanup job for old challenges.

## Authorization (ID-06)

### JWT contract

All protected services use the same HS256 JWT contract and the shared `FitnessApp.Auth` library:

| Claim | Value |
|---|---|
| `sub` | Guid user id (parsed by `ICurrentUser`) |
| `role` | Exactly one role: `User` or `Admin` (missing/invalid role => 403 on admin endpoints) |
| `iss` | `Jwt:Issuer` |
| `aud` | `Jwt:Audience` |
| `exp` | Expiry (missing/invalid/expired token => 401) |

`MapInboundClaims = false`, so claim names stay exactly as above. `RoleClaimType = "role"` and
`NameClaimType = "sub"`. Clock skew is 30 seconds. The fallback policy requires an authenticated
user; the `AdminOnly` policy additionally requires `role == "Admin"`.

Configuration is validated at startup:

```bash
# .env (git-ignored) - at least 32 characters
JWT_SIGNING_KEY=<generate with: openssl rand -base64 48>
```

`docker-compose.yml` passes `Jwt__Issuer=fitnessapp-team1`,
`Jwt__Audience=fitnessapp-team1-clients` and `Jwt__SigningKey=${JWT_SIGNING_KEY:?...}` to
IdentityService and TrainingService. When running outside Docker, set `Jwt__Issuer`,
`Jwt__Audience` and `Jwt__SigningKey` with environment variables or user-secrets. No secret is
committed.

### Dev token (Development only)

Because ID-01/ID-03 do not exist yet, IdentityService exposes a temporary token issuer:

```bash
curl -s -X POST http://localhost:5190/api/dev/token \
  -H 'Content-Type: application/json' \
  -d '{"userId":"00000000-0000-0000-0000-000000000001","role":"Admin"}'
# use the returned access_token as: Authorization: Bearer <access_token>
```

Omit `userId` to get a random Guid; omit `role` to get `User`. Tokens last one hour.
**TODO: remove `/api/dev/token` when ID-03 login issues real tokens.**

### Endpoint policy map

| Endpoint | Policy |
|---|---|
| IdentityService `POST /api/dev/token` (Development only) | Anonymous (temporary) |
| TrainingService OpenAPI JSON (Development only) | Anonymous |
| TrainingService `GET /api/authz/admin-ping` (Development/Test only) | AdminOnly |
| TrainingService `GET /api/authz/user-ping` (Development/Test only) | Authenticated |
| TrainingService `GET /api/dev/catalog/exercises` (Development/Test only) | Authenticated |
| TrainingService `POST /api/dev/catalog/exercises` (Development/Test only) | AdminOnly |
| TrainingService `GET /api/dev/sessions`, `GET/PUT/DELETE /api/dev/sessions/{id}` (Development/Test only) | Authenticated + current-user filter |
| Catalog GETs (exercises, workout templates, programs) when implemented | Authenticated |
| Catalog mutations and publish/archive when implemented | AdminOnly |

TrainingService has no product endpoints yet. The `/api/authz/*` and `/api/dev/*` routes are
**authorization-only test endpoints**, not new product features. Real endpoints must apply the
same policies.

Personal data (`WorkoutSession`, `UserTrainingProgram`, derived history) must never trust a user
id from a route/query/body. Use `OwnedQueries.ForCurrentUser(ICurrentUser)` for every query and
load by id through that filter; a missing row returns 404. Admin does **not** get cross-user
access. Rejections happen before any write.

### Notes for future work

- ID-01/ID-03: create new accounts with role `Roles.Default` (`"User"`) and include `sub` and
  `role` claims in issued tokens. Do not add registration or login here.
- PaymentService, ProgressService, NutritionService and Fitness_Profile_Service are still empty.
  When they gain endpoints they must call `builder.Services.AddFitnessAuth(builder.Configuration)`
  and add `app.UseAuthentication()` / `app.UseAuthorization()` before endpoint mapping.
