using System.ComponentModel.DataAnnotations;
using IdentityService.Application.EmailVerification;
using IdentityService.Infrastructure.Email;

namespace IdentityService.Api;

public sealed record VerifyEmailRequest(string? Email, string? Code);
public sealed record ResendVerificationRequest(string? Email);
public sealed record FakeRegisterRequest([property: Required] string Email);

public static class EmailVerificationEndpoints
{
    public static IEndpointRouteBuilder MapEmailVerificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").AllowAnonymous();

        group.MapPost("/verify-email", async (VerifyEmailRequest req, IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Code))
                return Failure(VerificationError.CodeInvalid);

            var result = await svc.VerifyAsync(req.Email, req.Code, ct);
            return result.Succeeded
                ? Results.Ok(new { message = "Email verified." })
                : Failure(result.Error!.Value);
        });

        group.MapPost("/resend-verification", async (ResendVerificationRequest req, IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (!string.IsNullOrWhiteSpace(req.Email))
                await svc.ResendAsync(req.Email, ct);

            // Same response whether or not the email exists / is confirmed / is in cooldown.
            return Results.Accepted(value: new
            {
                message = "If the email is registered and not yet verified, a new code has been sent."
            });
        });

        return app;
    }

    /// <summary>Development only: stands in for registration until ID-01 is wired.</summary>
    public static IEndpointRouteBuilder MapDevFakeRegistration(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/dev/fake-register", async (FakeRegisterRequest req, InMemoryUserEmailStore store,
            IEmailVerificationService svc, CancellationToken ct) =>
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Email))
                return Results.BadRequest(new { error = "email_required", detail = "A non-empty email is required." });

            var userId = store.Add(req.Email);
            await svc.IssueAsync(userId, req.Email, ct);
            return Results.Accepted();
        }).AllowAnonymous();
        return app;
    }

    private static IResult Failure(VerificationError error)
    {
        var (status, code, detail) = error switch
        {
            VerificationError.CodeExpired => (400, "code_expired", "The verification code has expired. Please request a new one."),
            VerificationError.CodeLocked => (429, "code_locked", "Too many incorrect attempts. Please request a new code."),
            VerificationError.CodeAlreadyUsed => (409, "code_already_used", "This verification code has already been used."),
            _ => (400, "code_invalid", "The verification code is invalid.")
        };

        return Results.Problem(
            title: "Email verification failed",
            detail: detail,
            statusCode: status,
            extensions: new Dictionary<string, object?> { ["code"] = code });
    }
}
