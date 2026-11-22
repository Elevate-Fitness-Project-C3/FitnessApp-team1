using IdentityService.Application.EmailVerification;
using Microsoft.Extensions.Logging;

namespace IdentityService.Infrastructure.Email;

/// <summary>Fake sender. Deliberately does NOT log the code.</summary>
public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendVerificationCodeAsync(string toEmail, string code, CancellationToken ct = default)
    {
        logger.LogInformation("Verification email would be sent to {Email} (code not logged)", toEmail);
        return Task.CompletedTask;
    }
}
