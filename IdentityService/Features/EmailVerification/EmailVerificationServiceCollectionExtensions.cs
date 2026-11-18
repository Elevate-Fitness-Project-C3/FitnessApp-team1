using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IdentityService.Features.EmailVerification;

public static class EmailVerificationServiceCollectionExtensions
{
    public static IServiceCollection AddEmailVerification(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<VerificationOptions>()
            .Bind(config.GetSection(VerificationOptions.SectionName))
            .Validate(o => o.HmacKey.Length >= 32, "Verification:HmacKey must be at least 32 characters.")
            .Validate(o => o.CodeLength is >= 4 and <= 9, "Verification:CodeLength must be between 4 and 9.")
            .Validate(o => o.ExpiryMinutes > 0, "Verification:ExpiryMinutes must be > 0.")
            .Validate(o => o.MaxAttempts > 0, "Verification:MaxAttempts must be > 0.")
            .Validate(o => o.ResendCooldownSeconds >= 0, "Verification:ResendCooldownSeconds must be >= 0.")
            .ValidateOnStart();

        services.AddOptions<EmailOptions>()
            .Bind(config.GetSection(EmailOptions.SectionName))
            .Validate(o => o.Provider == "Logging" || !string.IsNullOrWhiteSpace(o.Smtp.Host),
                "Email:Smtp:Host is required when Email:Provider is Smtp.")
            .ValidateOnStart();

        services.AddDbContext<EmailVerificationDbContext>(o =>
            o.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.TryAddSingleton(TimeProvider.System);

        services.AddSingleton<InMemoryUserEmailStore>();
        services.AddSingleton<IUserEmailStore>(sp => sp.GetRequiredService<InMemoryUserEmailStore>());

        var provider = config.GetSection(EmailOptions.SectionName)["Provider"];
        if (string.Equals(provider, "Logging", StringComparison.OrdinalIgnoreCase))
            services.AddSingleton<IEmailSender, LoggingEmailSender>();
        else
            services.AddSingleton<IEmailSender, SmtpEmailSender>();

        services.AddScoped<IEmailVerificationService, EmailVerificationService>();
        return services;
    }
}
