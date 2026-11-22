using System.Collections.Concurrent;
using IdentityService.Application.EmailVerification;

namespace IdentityService.Infrastructure.Email;

/// <summary>Fake user store so ID-02 works before ID-01 is wired. Replace with the real one.</summary>
public sealed class InMemoryUserEmailStore : IUserEmailStore
{
    private readonly ConcurrentDictionary<string, UserEmailInfo> _byEmail = new();

    public Guid Add(string email)
    {
        var normalized = Normalize(email);
        var id = Guid.NewGuid();
        _byEmail[normalized] = new UserEmailInfo(id, normalized, false);
        return id;
    }

    public bool IsConfirmed(string email) =>
        _byEmail.TryGetValue(Normalize(email), out var u) && u.EmailConfirmed;

    public Task<UserEmailInfo?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_byEmail.TryGetValue(Normalize(email), out var u) ? u : null);

    public Task MarkEmailConfirmedAsync(Guid userId, CancellationToken ct = default)
    {
        foreach (var (key, u) in _byEmail)
            if (u.UserId == userId) _byEmail[key] = u with { EmailConfirmed = true };
        return Task.CompletedTask;
    }

    private static string Normalize(string email) => email.Trim().ToLowerInvariant();
}
