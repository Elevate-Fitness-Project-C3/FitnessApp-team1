using Xunit;
using IdentityService.Application.EmailVerification;
using IdentityService.Infrastructure.Email;
using IdentityService.Infrastructure.Persistence.Contexts;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using MsOptions = Microsoft.Extensions.Options.Options;

namespace IdentityService.Tests;

public sealed class CapturingEmailSender : IEmailSender
{
    public List<(string To, string Code)> Sent { get; } = [];
    public string LastCode => Sent[^1].Code;

    public Task SendVerificationCodeAsync(string toEmail, string code, string body, CancellationToken ct = default)
    {
        lock (Sent) Sent.Add((toEmail, code));
        return Task.CompletedTask;
    }
}

/// <summary>
/// File-based SQLite so several DbContexts (= several parallel requests) hit one real database.
/// </summary>
public sealed class TestEnv : IDisposable
{
    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"verif-{Guid.NewGuid():N}.db");

    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 11, 15, 12, 0, 0, TimeSpan.Zero));
    public InMemoryUserEmailStore Users { get; } = new();
    public CapturingEmailSender Sender { get; } = new();
    public VerificationOptions Opts { get; } = new() { HmacKey = new string('k', 48) };

    public TestEnv()
    {
        using var db = OpenDb();
        db.Database.EnsureCreated();
    }

    public EmailVerificationDbContext OpenDb() =>
        new(new DbContextOptionsBuilder<EmailVerificationDbContext>().UseSqlite($"Data Source={_dbPath}").Options);

    public EmailVerificationService NewService() =>
        new(OpenDb(), Users, Sender, MsOptions.Create(Opts), Time, NullLogger<EmailVerificationService>.Instance);

    public async Task<string> RegisterAndIssueAsync(string email)
    {
        var id = Users.Add(email);
        await NewService().IssueAsync(id, email);
        return Sender.LastCode;
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        try { File.Delete(_dbPath); } catch { /* best effort */ }
    }
}

public class EmailVerificationServiceTests : IDisposable
{
    private const string Email = "fatima@example.com";
    private readonly TestEnv _env = new();

    public void Dispose() => _env.Dispose();

    private static string Wrong(string code) => code == "000000" ? "111111" : "000000";

    [Fact]
    public async Task Verify_with_correct_code_confirms_email()
    {
        var code = await _env.RegisterAndIssueAsync(Email);

        var result = await _env.NewService().VerifyAsync(Email, code);

        Assert.True(result.Succeeded);
        Assert.True(_env.Users.IsConfirmed(Email));
    }

    [Fact]
    public async Task Used_code_cannot_be_replayed()
    {
        var code = await _env.RegisterAndIssueAsync(Email);
        await _env.NewService().VerifyAsync(Email, code);

        var replay = await _env.NewService().VerifyAsync(Email, code);

        Assert.False(replay.Succeeded);
        Assert.Equal(VerificationError.CodeAlreadyUsed, replay.Error);
    }

    [Fact]
    public async Task Expired_code_is_rejected()
    {
        var code = await _env.RegisterAndIssueAsync(Email);
        _env.Time.Advance(TimeSpan.FromMinutes(11));

        var result = await _env.NewService().VerifyAsync(Email, code);

        Assert.Equal(VerificationError.CodeExpired, result.Error);
        Assert.False(_env.Users.IsConfirmed(Email));
    }

    [Fact]
    public async Task Wrong_code_increments_attempts()
    {
        var code = await _env.RegisterAndIssueAsync(Email);

        var result = await _env.NewService().VerifyAsync(Email, Wrong(code));

        Assert.Equal(VerificationError.CodeInvalid, result.Error);
        await using var db = _env.OpenDb();
        Assert.Equal(1, await db.Challenges.Select(c => c.AttemptCount).SingleAsync());
    }

    [Fact]
    public async Task Challenge_locks_after_five_wrong_attempts_even_for_the_right_code()
    {
        var code = await _env.RegisterAndIssueAsync(Email);

        for (var i = 1; i <= 4; i++)
            Assert.Equal(VerificationError.CodeInvalid, (await _env.NewService().VerifyAsync(Email, Wrong(code))).Error);

        Assert.Equal(VerificationError.CodeLocked, (await _env.NewService().VerifyAsync(Email, Wrong(code))).Error);
        Assert.Equal(VerificationError.CodeLocked, (await _env.NewService().VerifyAsync(Email, code)).Error);
        Assert.False(_env.Users.IsConfirmed(Email));
    }

    [Fact]
    public async Task Resend_invalidates_older_unused_challenges()
    {
        await _env.RegisterAndIssueAsync(Email);
        _env.Time.Advance(TimeSpan.FromSeconds(61));

        await _env.NewService().ResendAsync(Email);

        await using var db = _env.OpenDb();
        var all = await db.Challenges.AsNoTracking().OrderBy(c => c.CreatedAt).ToListAsync();
        Assert.Equal(2, all.Count);
        Assert.NotNull(all[0].InvalidatedAt);
        Assert.Null(all[1].InvalidatedAt);
        Assert.Single(all, c => c.InvalidatedAt == null && c.ConsumedAt == null);
    }

    [Fact]
    public async Task Old_code_fails_after_resend_and_new_code_works()
    {
        var oldCode = await _env.RegisterAndIssueAsync(Email);
        _env.Time.Advance(TimeSpan.FromSeconds(61));
        await _env.NewService().ResendAsync(Email);
        var newCode = _env.Sender.LastCode;
        Assert.NotEqual(oldCode, newCode);

        var oldResult = await _env.NewService().VerifyAsync(Email, oldCode);
        var newResult = await _env.NewService().VerifyAsync(Email, newCode);

        Assert.Equal(VerificationError.CodeInvalid, oldResult.Error);
        Assert.True(newResult.Succeeded);
    }

    [Fact]
    public async Task Resend_within_cooldown_does_not_send_again()
    {
        await _env.RegisterAndIssueAsync(Email);

        await _env.NewService().ResendAsync(Email);

        Assert.Single(_env.Sender.Sent);
    }

    [Fact]
    public async Task Unknown_email_gets_generic_failure_and_resend_does_not_throw()
    {
        var verify = await _env.NewService().VerifyAsync("nobody@example.com", "123456");
        await _env.NewService().ResendAsync("nobody@example.com");

        Assert.Equal(VerificationError.CodeInvalid, verify.Error);
        Assert.Empty(_env.Sender.Sent);
    }

    [Fact]
    public async Task Concurrent_verify_lets_only_one_request_succeed()
    {
        var code = await _env.RegisterAndIssueAsync(Email);

        var tasks = Enumerable.Range(0, 2)
            .Select(_ => Task.Run(() => _env.NewService().VerifyAsync(Email, code)))
            .ToArray();
        var results = await Task.WhenAll(tasks);

        Assert.Equal(1, results.Count(r => r.Succeeded));
        Assert.Equal(VerificationError.CodeAlreadyUsed, results.Single(r => !r.Succeeded).Error);
    }
}
