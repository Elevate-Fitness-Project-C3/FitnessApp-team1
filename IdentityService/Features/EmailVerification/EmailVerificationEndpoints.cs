namespace IdentityService.Features.EmailVerification;

public sealed record VerifyEmailRequest(string? Email, string? Code);
public sealed record ResendVerificationRequest(string? Email);
public sealed record FakeRegisterRequest(string? Email);

public static class EmailVerificationEndpoints
{
    public static IEndpointRouteBuilder MapEmailVerificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/verify-email", async (VerifyEmailRequest req, IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Code))
                return Failure("code_invalid", "The verification code is invalid.");

            var outcome = await svc.VerifyAsync(req.Email, req.Code, ct);

            return outcome switch
            {
                VerifyOutcome.Success => Results.Ok(new { message = "Email verified." }),
                VerifyOutcome.CodeExpired => Failure("code_expired", "The verification code has expired. Please request a new one."),
                VerifyOutcome.CodeLocked => Failure("code_locked", "Too many incorrect attempts. Please request a new code."),
                VerifyOutcome.CodeAlreadyUsed => Failure("code_already_used", "This verification code has already been used."),
                _ => Failure("code_invalid", "The verification code is invalid.")
            };
        });

        group.MapPost("/resend-verification", async (ResendVerificationRequest req, IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (!string.IsNullOrWhiteSpace(req.Email))
                await svc.ResendAsync(req.Email, ct);

            // Same answer whether the email exists or not (no user enumeration).
            return Results.Accepted(value: new { message = "If the account exists and is not verified, a new code has been sent." });
        });

        return app;
    }

    /// <summary>DEV ONLY: creates a fake user + sends a code. Delete when ID-01 is wired.</summary>
    public static IEndpointRouteBuilder MapDevFakeRegistration(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dev/fake-register", async (FakeRegisterRequest req, InMemoryUserEmailStore store, IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email)) return Results.BadRequest();
            var user = store.Add(req.Email);
            await svc.IssueAsync(user.UserId, user.Email, ct);
            return Results.Accepted(value: new { message = "Fake user created. Check MailHog." });
        });
        return app;
    }

    private static IResult Failure(string code, string detail) =>
        Results.Problem(
            statusCode: StatusCodes.Status400BadRequest,
            title: "Email verification failed",
            detail: detail,
            extensions: new Dictionary<string, object?> { ["code"] = code });
}
