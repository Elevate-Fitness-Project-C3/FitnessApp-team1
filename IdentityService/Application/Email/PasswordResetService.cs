using IdentityService.Application.Emails;
using IdentityService.Application.Sessions;
using IdentityService.Domain;
using IdentityService.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IdentityService.Application;

public enum ResetOutcome { Success, InvalidToken, WeakPassword }

public sealed record ResetResult(ResetOutcome Outcome, string[]? PasswordErrors = null);

public sealed class PasswordResetService(
    AppDbContext db,
    ResetPasswordDbContext resetDb,
    IEmailQueue emails,
    IPasswordHasher<AppUser> hasher,
    IRefreshSessionRevoker refreshSessionRevoker,
    TimeProvider time,
    IOptions<PasswordResetOptions> options)
{
    private readonly PasswordResetOptions _opts = options.Value;

    public async Task RequestAsync(string email, CancellationToken ct)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalized, ct);
        if (user is null || string.IsNullOrWhiteSpace(user.Email)) return;

        var now = time.GetUtcNow().UtcDateTime;

        await resetDb.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now), ct);

        var (raw, hash) = ResetTokenGenerator.Generate();
        resetDb.PasswordResetTokens.Add(
            PasswordResetToken.Create(user.Id, hash, now, TimeSpan.FromMinutes(_opts.LifetimeMinutes)));
        await resetDb.SaveChangesAsync(ct);

        var link = $"{_opts.ResetUrlBase}?email={Uri.EscapeDataString(user.Email)}&token={raw}";
        emails.Enqueue(new QueuedEmail(
            user.Email,
            "Reset your password",
            $"Use this link to reset your password. It expires in {_opts.LifetimeMinutes} minutes:\n{link}\n\n" +
            "If you did not request this, ignore this email."));
    }

    public async Task<ResetResult> ResetAsync(string email, string token, string newPassword, CancellationToken ct)
    {
        var passwordErrors = PasswordPolicy.Validate(newPassword);
        if (passwordErrors.Length > 0)
            return new ResetResult(ResetOutcome.WeakPassword, passwordErrors);   // token is NOT consumed

        var now = time.GetUtcNow().UtcDateTime;
        var normalized = email.Trim().ToLowerInvariant();
        var hash = ResetTokenGenerator.Hash(token);

        var user = await db.Users.FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == normalized, ct);
        if (user is null) return new ResetResult(ResetOutcome.InvalidToken);

        var record = await resetDb.PasswordResetTokens
            .FirstOrDefaultAsync(t => t.UserId == user.Id && t.TokenHash == hash, ct);
        if (record is null || !record.IsUsable(now)) return new ResetResult(ResetOutcome.InvalidToken);

        var claimed = await resetDb.PasswordResetTokens
            .Where(t => t.Id == record.Id && t.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now), ct);
        if (claimed == 0) return new ResetResult(ResetOutcome.InvalidToken);

        user.PasswordHash = hasher.HashPassword(user, newPassword);
        await db.SaveChangesAsync(ct);

        await resetDb.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now), ct);

        await refreshSessionRevoker.RevokeAllAsync(user.Id, now, ct);

        return new ResetResult(ResetOutcome.Success);
    }
}