namespace IdentityService.Domain;

/// <summary>One verification attempt-set for a user. The raw code is never stored, only its HMAC.</summary>
public sealed class EmailVerificationChallenge
{
    private EmailVerificationChallenge() { } // EF Core

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public byte[] CodeHash { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public int AttemptCount { get; private set; }
    public DateTime? ConsumedAt { get; private set; }
    public DateTime? InvalidatedAt { get; private set; }

    public static EmailVerificationChallenge Create(
        Guid id, Guid userId, byte[] codeHash, DateTime createdAtUtc, DateTime expiresAtUtc) => new()
    {
        Id = id,
        UserId = userId,
        CodeHash = codeHash,
        CreatedAt = createdAtUtc,
        ExpiresAt = expiresAtUtc
    };
}
