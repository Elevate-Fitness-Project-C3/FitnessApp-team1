using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FitnessProfileService.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by `dotnet ef migrations ...`.
/// It never needs a real password because migrations are generated from the model,
/// not from a live database. Runtime configuration wins from environment/user-secrets.
/// </summary>
public sealed class FitnessProfileDbContextFactory : IDesignTimeDbContextFactory<FitnessProfileDbContext>
{
    public FitnessProfileDbContext CreateDbContext(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(local);Database=FitnessProfileDb;Trusted_Connection=True;TrustServerCertificate=True";

        var options = new DbContextOptionsBuilder<FitnessProfileDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new FitnessProfileDbContext(options);
    }
}
