using FitnessApp.Auth;
using FitnessProfileService.Application;
using FitnessProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Api;

public static class ProfileServiceCollectionExtensions
{
    public static IServiceCollection AddFitnessProfileServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing for FitnessProfileService.");

        services.AddFitnessAuth(configuration);
        services.AddDbContext<FitnessProfileDbContext>(options => options.UseSqlServer(connectionString));

        services.AddSingleton(TimeProvider.System);
        services.AddScoped<ProfileService>();
        services.AddScoped<IOnboardingGate>(sp => sp.GetRequiredService<ProfileService>());

        return services;
    }
}
