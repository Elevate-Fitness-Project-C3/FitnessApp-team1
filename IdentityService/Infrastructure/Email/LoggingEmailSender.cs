using IdentityService.Application.EmailVerification;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Email;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendVerificationCodeAsync(string toEmail, string code, string body, CancellationToken ct = default)
    {
        logger.LogInformation("Verification email would be sent to {Email} (code not logged)", toEmail);
        return Task.CompletedTask;
    }

    public Task SendAsync(string toEmail, string subject, string body, CancellationToken ct = default)
    {
        logger.LogInformation("Email '{Subject}' would be sent to {Email}", subject, toEmail);
        return Task.CompletedTask;
    }
}
