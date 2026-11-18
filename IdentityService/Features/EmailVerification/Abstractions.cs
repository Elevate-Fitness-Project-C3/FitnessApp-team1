namespace IdentityService.Features.EmailVerification;

public sealed record UserEmailInfo(Guid UserId, string Email, bool EmailConfirmed);

/// <summary>Swap this for the real user store when ID-01 (registration) is ready.</summary>
public interface IUserEmailStore
{
    Task<UserEmailInfo?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task MarkEmailConfirmedAsync(Guid userId, CancellationToken ct = default);
}

public interface IEmailSender
{
    Task SendVerificationCodeAsync(string toEmail, string code, CancellationToken ct = default);
}

public enum VerifyOutcome
{
    Success,
    CodeInvalid,
    CodeExpired,
    CodeLocked,
    CodeAlreadyUsed
}

public interface IEmailVerificationService
{
    /// <summary>The ONE method the registration flow calls after saving the user.</summary>
    Task IssueAsync(Guid userId, string email, CancellationToken ct = default);

    Task<VerifyOutcome> VerifyAsync(string email, string code, CancellationToken ct = default);

    /// <summary>Never reveals whether the email exists or whether the cooldown blocked it.</summary>
    Task ResendAsync(string email, CancellationToken ct = default);
}
