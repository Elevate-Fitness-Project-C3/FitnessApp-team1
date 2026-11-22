using IdentityService.Application.EmailVerification;
using IdentityService.Infrastructure.Email;
using IdentityService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Api;

public static class EmailVerificationServiceCollectionExtensions
{
    public static IServiceCollection AddEmailVerification(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<VerificationOptions>()
            .Bind(config.GetSection(VerificationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(config.GetSection(EmailOptions.SectionName))
            .Validate(o => !string.IsNullOrWhiteSpace(o.From) && !string.IsNullOrWhiteSpace(o.Smtp.Host) && o.Smtp.Port > 0,
                "Email:From, Email:Smtp:Host and Email:Smtp:Port are required.")
            .ValidateOnStart();

        services.TryAddSingleton(TimeProvider.System);

        services.AddDbContext<EmailVerificationDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory_Verification")));

        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>(); // swap for LoggingEmailSender if you want no SMTP

        // Fake user store: replace with the real one when wiring registration.
        services.AddSingleton<InMemoryUserEmailStore>();
        services.AddSingleton<IUserEmailStore>(sp => sp.GetRequiredService<InMemoryUserEmailStore>());

        return services;
    }
}
