using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityService.Features.EmailVerification;

public sealed class EmailVerificationService(
    EmailVerificationDbContext db,
    IUserEmailStore users,
    IEmailSender sender,
    IOptions<VerificationOptions> options,
    TimeProvider time,
    ILogger<EmailVerificationService> logger) : IEmailVerificationService
{
    private readonly VerificationOptions _o = options.Value;

    public async Task IssueAsync(Guid userId, string email, CancellationToken ct = default)
    {
        var code = GenerateCode();
        var now = time.GetUtcNow().UtcDateTime;

        var challenge = new EmailVerificationChallenge
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CreatedAt = now,
            ExpiresAt = now.AddMinutes(_o.ExpiryMinutes)
        };
        challenge.CodeHash = Hash(challenge.Id, code);

        // One transaction: invalidate old unused challenges, then create the new one.
        await using (var tx = await db.Database.BeginTransactionAsync(ct))
        {
            await db.Challenges
                .Where(c => c.UserId == userId && c.ConsumedAt == null && c.InvalidatedAt == null)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.InvalidatedAt, now), ct);

            db.Challenges.Add(challenge);
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }

        // Email failure must not break registration: the user can use "resend".
        try
        {
            await sender.SendVerificationCodeAsync(email, code, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send verification email for user {UserId}", userId);
        }
    }

    public async Task ResendAsync(string email, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(email, ct);
        if (user is null || user.EmailConfirmed) return;

        var now = time.GetUtcNow().UtcDateTime;
        var last = await db.Challenges
            .AsNoTracking()
            .Where(c => c.UserId == user.UserId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (last is not null && now - last.CreatedAt < TimeSpan.FromSeconds(_o.ResendCooldownSeconds))
            return; // still in cooldown: silently ignore

        await IssueAsync(user.UserId, user.Email, ct);
    }

    public async Task<VerifyOutcome> VerifyAsync(string email, string code, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(email, ct);
        if (user is null) return VerifyOutcome.CodeInvalid;

        var now = time.GetUtcNow().UtcDateTime;
        var max = _o.MaxAttempts;

        var ch = await db.Challenges
            .AsNoTracking()
            .Where(c => c.UserId == user.UserId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (ch is null) return VerifyOutcome.CodeInvalid;
        if (ch.ConsumedAt is not null) return VerifyOutcome.CodeAlreadyUsed;
        if (ch.InvalidatedAt is not null) return VerifyOutcome.CodeInvalid;
        if (ch.ExpiresAt <= now) return VerifyOutcome.CodeExpired;
        if (ch.AttemptCount >= max) return VerifyOutcome.CodeLocked;

        var matches = CryptographicOperations.FixedTimeEquals(Hash(ch.Id, (code ?? string.Empty).Trim()), ch.CodeHash);

        if (!matches)
        {
            // Atomic increment (safe for parallel requests).
            await db.Challenges
                .Where(c => c.Id == ch.Id && c.ConsumedAt == null && c.AttemptCount < max)
                .ExecuteUpdateAsync(s => s.SetProperty(c => c.AttemptCount, c => c.AttemptCount + 1), ct);

            var count = await db.Challenges.AsNoTracking()
                .Where(c => c.Id == ch.Id)
                .Select(c => c.AttemptCount)
                .SingleAsync(ct);

            return count >= max ? VerifyOutcome.CodeLocked : VerifyOutcome.CodeInvalid;
        }

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // The WHERE is what makes "use once" safe: only ONE parallel request can flip ConsumedAt.
        var affected = await db.Challenges
            .Where(c => c.Id == ch.Id
                        && c.ConsumedAt == null
                        && c.InvalidatedAt == null
                        && c.AttemptCount < max
                        && c.ExpiresAt > now)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ConsumedAt, now), ct);

        if (affected != 1) return VerifyOutcome.CodeAlreadyUsed; // lost the race -> rollback

        await users.MarkEmailConfirmedAsync(user.UserId, ct);
        await tx.CommitAsync(ct);
        return VerifyOutcome.Success;
    }

    private string GenerateCode()
    {
        var upper = (int)Math.Pow(10, _o.CodeLength);
        return RandomNumberGenerator.GetInt32(0, upper).ToString($"D{_o.CodeLength}");
    }

    // Bound to the challenge id, so a hash can't be reused for another challenge.
    private byte[] Hash(Guid challengeId, string code) =>
        HMACSHA256.HashData(
            Encoding.UTF8.GetBytes(_o.HmacKey),
            Encoding.UTF8.GetBytes($"{challengeId:N}:{code}"));
}
