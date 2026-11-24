using System.Security.Cryptography;
using System.Text;
using IdentityService.Domain;
using IdentityService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityService.Application.EmailVerification;

public sealed class EmailVerificationService(
    EmailVerificationDbContext db,
    IUserEmailStore users,
    IEmailSender emailSender,
    IOptions<VerificationOptions> options,
    TimeProvider time,
    ILogger<EmailVerificationService> logger) : IEmailVerificationService
{
    private readonly VerificationOptions _opt = options.Value;

    // ---------- Issue / Resend ----------

    public async Task IssueAsync(Guid userId, string email, CancellationToken ct = default)
    {
        var code = await CreateChallengeAsync(userId, ct);
        if (code is null) return;
        await SendSafelyAsync(userId, email, code, ct);
    }

    public async Task ResendAsync(string email, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(Normalize(email), ct);
        if (user is null || user.EmailConfirmed) return; // silent: no user enumeration

        var latest = await GetLatestAsync(user.UserId, ct);
        if (latest is not null &&
            UtcNow() < latest.CreatedAt.AddSeconds(_opt.ResendCooldownSeconds))
            return; // cooldown: silent, same response as success

        await IssueAsync(user.UserId, user.Email, ct);
    }

    /// <summary>Invalidates older unused challenges and creates a new one, in one transaction.</summary>
    private async Task<string?> CreateChallengeAsync(Guid userId, CancellationToken ct)
    {
        var now = UtcNow();
        var code = GenerateCode();
        var id = Guid.NewGuid();

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        await db.Challenges
            .Where(c => c.UserId == userId && c.ConsumedAt == null && c.InvalidatedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.InvalidatedAt, (DateTime?)now), ct);

        db.Challenges.Add(EmailVerificationChallenge.Create(
            id, userId, Hash(id, code), now, now.AddMinutes(_opt.ExpiryMinutes)));

        try
        {
            await db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
        }
        catch (DbUpdateException ex)
        {
            // Filtered unique index: a parallel issue for the same user won the race.
            logger.LogWarning(ex, "Concurrent verification issue for user {UserId}", userId);
            return null;
        }

        return code;
    }

    private async Task SendSafelyAsync(Guid userId, string email, string code, CancellationToken ct)
    {
        try
        {
            await emailSender.SendVerificationCodeAsync(email, code, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Registration must not fail because SMTP is down; the user can resend. Never log the code.
            logger.LogError(ex, "Failed to send verification email for user {UserId}", userId);
        }
    }

    // ---------- Verify ----------

    public async Task<VerifyResult> VerifyAsync(string email, string code, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(Normalize(email), ct);
        if (user is null || !IsWellFormed(code))
            return VerifyResult.Fail(VerificationError.CodeInvalid);

        var challenge = await GetLatestAsync(user.UserId, ct);
        if (challenge is null)
            return VerifyResult.Fail(VerificationError.CodeInvalid);

        var now = UtcNow();
        var blocked = Classify(challenge, now);
        if (blocked is not null)
            return VerifyResult.Fail(blocked.Value);

        var matches = CryptographicOperations.FixedTimeEquals(challenge.CodeHash, Hash(challenge.Id, code));
        return matches
            ? await ConsumeAsync(user, challenge.Id, now, ct)
            : await RegisterFailedAttemptAsync(challenge.Id, ct);
    }

    private async Task<VerifyResult> RegisterFailedAttemptAsync(Guid challengeId, CancellationToken ct)
    {
        var max = _opt.MaxAttempts;

        // Atomic increment; never goes past max and never touches consumed/invalidated rows.
        await db.Challenges
            .Where(c => c.Id == challengeId && c.ConsumedAt == null && c.InvalidatedAt == null && c.AttemptCount < max)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.AttemptCount, c => c.AttemptCount + 1), ct);

        var attempts = await db.Challenges.AsNoTracking()
            .Where(c => c.Id == challengeId).Select(c => c.AttemptCount).SingleAsync(ct);

        return VerifyResult.Fail(attempts >= max ? VerificationError.CodeLocked : VerificationError.CodeInvalid);
    }

    private async Task<VerifyResult> ConsumeAsync(UserEmailInfo user, Guid challengeId, DateTime now, CancellationToken ct)
    {
        var max = _opt.MaxAttempts;

        await using var tx = await db.Database.BeginTransactionAsync(ct);

        // Conditional update = the "only once" guarantee. Of two parallel requests, only one gets rows == 1.
        var rows = await db.Challenges
            .Where(c => c.Id == challengeId && c.ConsumedAt == null && c.InvalidatedAt == null
                        && c.AttemptCount < max && c.ExpiresAt > now)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.ConsumedAt, (DateTime?)now), ct);

        if (rows == 0)
        {
            await tx.RollbackAsync(ct);
            var current = await db.Challenges.AsNoTracking().SingleOrDefaultAsync(c => c.Id == challengeId, ct);
            var error = current is null ? VerificationError.CodeInvalid : Classify(current, UtcNow());
            return VerifyResult.Fail(error ?? VerificationError.CodeInvalid);
        }

        await users.MarkEmailConfirmedAsync(user.UserId, ct);
        await tx.CommitAsync(ct);
        return VerifyResult.Success;
    }

    // ---------- Helpers ----------

    private VerificationError? Classify(EmailVerificationChallenge c, DateTime now)
    {
        if (c.ConsumedAt is not null) return VerificationError.CodeAlreadyUsed;
        if (c.InvalidatedAt is not null) return VerificationError.CodeInvalid;
        if (c.AttemptCount >= _opt.MaxAttempts) return VerificationError.CodeLocked;
        if (now >= c.ExpiresAt) return VerificationError.CodeExpired;
        return null;
    }

    private Task<EmailVerificationChallenge?> GetLatestAsync(Guid userId, CancellationToken ct) =>
        db.Challenges.AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync(ct);

    private string GenerateCode() =>
        RandomNumberGenerator.GetInt32((int)Math.Pow(10, _opt.CodeLength)).ToString($"D{_opt.CodeLength}");

    private bool IsWellFormed(string? code) =>
        code is not null && code.Length == _opt.CodeLength && code.All(char.IsAsciiDigit);

    /// <summary>HMAC bound to the challenge id, so a hash can't be replayed against another challenge.</summary>
    private byte[] Hash(Guid challengeId, string code) =>
        HMACSHA256.HashData(Encoding.UTF8.GetBytes(_opt.HmacKey), Encoding.UTF8.GetBytes($"{challengeId:N}:{code}"));

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();

    private DateTime UtcNow() => time.GetUtcNow().UtcDateTime;
}
