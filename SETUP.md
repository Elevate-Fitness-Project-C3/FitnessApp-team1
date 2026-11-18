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
