namespace IdentityService.Application.EmailVerification;

public sealed record UserEmailInfo(Guid UserId, string Email, bool EmailConfirmed);

/// <summary>Bridge to the registration/users code (ID-01). Replace the in-memory fake later.</summary>
public interface IUserEmailStore
{
    Task<UserEmailInfo?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task MarkEmailConfirmedAsync(Guid userId, CancellationToken ct = default);
}

public interface IEmailSender
{
    Task SendVerificationCodeAsync(string toEmail, string code, string body, CancellationToken ct = default);
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default);
}

public enum VerificationError
{
    CodeInvalid,
    CodeExpired,
    CodeLocked,
    CodeAlreadyUsed
}

public sealed record VerifyResult(bool Succeeded, VerificationError? Error)
{
    public static VerifyResult Success { get; } = new(true, null);
    public static VerifyResult Fail(VerificationError error) => new(false, error);
}

public interface IEmailVerificationService
{
    /// <summary>The single method registration will call after creating the user.</summary>
    Task IssueAsync(Guid userId, string email, CancellationToken ct = default);

    Task<VerifyResult> VerifyAsync(string email, string code, CancellationToken ct = default);

    /// <summary>Never reveals whether the email exists / is confirmed / is in cooldown.</summary>
    Task ResendAsync(string email, CancellationToken ct = default);
}
