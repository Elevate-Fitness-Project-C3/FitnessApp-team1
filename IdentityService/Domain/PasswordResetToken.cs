namespace IdentityService.Domain;

public sealed class PasswordResetToken
{
    private PasswordResetToken() { }

    public Guid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public string TokenHash { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? UsedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public static PasswordResetToken Create(string userId, string tokenHash, DateTime nowUtc, TimeSpan lifetime) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        TokenHash = tokenHash,
        CreatedAt = nowUtc,
        ExpiresAt = nowUtc.Add(lifetime)
    };

    public bool IsUsable(DateTime nowUtc) => UsedAt is null && ExpiresAt > nowUtc;
}