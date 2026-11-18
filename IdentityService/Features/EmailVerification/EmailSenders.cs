using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Collections.Concurrent;

namespace IdentityService.Features.EmailVerification;

public sealed class SmtpEmailSender(IOptions<EmailOptions> email, IOptions<VerificationOptions> verification) : IEmailSender
{
    public async Task SendVerificationCodeAsync(string toEmail, string code, CancellationToken ct = default)
    {
        var o = email.Value;

        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(o.From));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = "Your verification code";
        msg.Body = new TextPart("plain")
        {
            Text = $"Your verification code is {code}. It expires in {verification.Value.ExpiryMinutes} minutes."
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            o.Smtp.Host, o.Smtp.Port,
            o.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None, ct);

        if (!string.IsNullOrEmpty(o.Smtp.User))
            await client.AuthenticateAsync(o.Smtp.User, o.Smtp.Password, ct);

        await client.SendAsync(FormatOptions.Default, new SendRequest(msg), ct);
        await client.DisconnectAsync(true, ct);
    }
}

/// <summary>Does not send anything and does NOT log the code.</summary>
public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendVerificationCodeAsync(string toEmail, string code, CancellationToken ct = default)
    {
        logger.LogInformation("A verification email was requested (code intentionally not logged).");
        return Task.CompletedTask;
    }
}

/// <summary>Temporary fake until ID-01 gives us real users. Forgets everything on restart.</summary>
public sealed class InMemoryUserEmailStore : IUserEmailStore
{
    private readonly ConcurrentDictionary<string, UserEmailInfo> _byEmail = new(StringComparer.OrdinalIgnoreCase);

    public UserEmailInfo Add(string email)
    {
        var info = new UserEmailInfo(Guid.NewGuid(), email, false);
        return _byEmail.AddOrUpdate(email, info, (_, existing) => existing);
    }

    public Task<UserEmailInfo?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        Task.FromResult(_byEmail.TryGetValue(email, out var u) ? u : null);

    public Task MarkEmailConfirmedAsync(Guid userId, CancellationToken ct = default)
    {
        foreach (var (key, u) in _byEmail)
            if (u.UserId == userId)
                _byEmail[key] = u with { EmailConfirmed = true };
        return Task.CompletedTask;
    }
}
