namespace IdentityService.Features.EmailVerification;

public sealed class EmailVerificationChallenge
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    /// <summary>HMAC-SHA256 of the code. The real code is never stored.</summary>
    public byte[] CodeHash { get; set; } = [];

    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? ConsumedAt { get; set; }
    public DateTime? InvalidatedAt { get; set; }
}
