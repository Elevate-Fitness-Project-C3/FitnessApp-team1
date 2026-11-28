using IdentityService.Application.Emails;
using IdentityService.Application.EmailVerification;

namespace IdentityService.Infrastructure.Emails;

public sealed class EmailBackgroundService(
    IEmailQueueReader queue,
    IServiceScopeFactory scopes,
    ILogger<EmailBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var mail in queue.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = scopes.CreateScope();
                var sender = scope.ServiceProvider.GetRequiredService<IEmailSender>();
                await sender.SendAsync(mail.To, mail.Subject, mail.Body, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {To}", mail.To);
            }
        }
    }
}