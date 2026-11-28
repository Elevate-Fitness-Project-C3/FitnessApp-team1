using IdentityService.Application.EmailVerification;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace IdentityService.Infrastructure.Email;

public sealed class SmtpEmailSender(IOptions<EmailOptions> email, IOptions<VerificationOptions> verification) : IEmailSender
{
    public async Task SendVerificationCodeAsync(string toEmail, string code, string body, CancellationToken ct = default)
    {
        await SendAsync(toEmail, "Your verification code", body, ct);
    }

    public async Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        var o = email.Value;

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(o.From));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(o.Smtp.Host, o.Smtp.Port, SecureSocketOptions.Auto, ct);
        if (!string.IsNullOrEmpty(o.Smtp.Username))
            await client.AuthenticateAsync(o.Smtp.Username, o.Smtp.Password ?? string.Empty, ct);
        await client.SendAsync(FormatOptions.Default, message, ct);
        await client.DisconnectAsync(true, ct);
    }
}
