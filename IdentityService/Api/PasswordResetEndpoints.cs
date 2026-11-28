using System.ComponentModel.DataAnnotations;
using IdentityService.Application;

namespace IdentityService.Api;

public sealed record ForgotPasswordRequest(string? Email);
public sealed record ResetPasswordRequest(string? Email, string? Token, string? NewPassword);

public static class PasswordResetEndpoints
{
    private const string GenericMessage = "If an account exists for this email, a reset link has been sent.";

    public static IEndpointRouteBuilder MapPasswordResetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth").AllowAnonymous();

        group.MapPost("/forgot-password", async (ForgotPasswordRequest request,
            PasswordResetService service, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email) || !new EmailAddressAttribute().IsValid(request.Email))
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["email"] = ["must be a valid email address."]
                });

            await service.RequestAsync(request.Email, ct);
            return Results.Ok(new { message = GenericMessage });   // same answer for every valid-looking email
        });

        group.MapPost("/reset-password", async (ResetPasswordRequest request,
            PasswordResetService service, CancellationToken ct) =>
        {
            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(request.Email)) errors["email"] = ["is required."];
            if (string.IsNullOrWhiteSpace(request.Token)) errors["token"] = ["is required."];
            if (string.IsNullOrWhiteSpace(request.NewPassword)) errors["newPassword"] = ["is required."];
            if (errors.Count > 0) return Results.ValidationProblem(errors);

            var result = await service.ResetAsync(request.Email!, request.Token!, request.NewPassword!, ct);

            return result.Outcome switch
            {
                ResetOutcome.Success => Results.Ok(new { message = "Password has been reset. Please log in." }),
                ResetOutcome.WeakPassword => Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["newPassword"] = result.PasswordErrors!
                }),
                _ => Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid or expired reset link",
                    detail: "The reset link is invalid, expired, or already used.",
                    extensions: new Dictionary<string, object?> { ["code"] = "invalid_reset_token" })
            };
        });

        return app;
    }
}