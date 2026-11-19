using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Tests;

internal static class TestConfig
{
    public const string Issuer = "test-issuer";
    public const string Audience = "test-audience";
    public const string SigningKey = "test-signing-key-that-is-definitely-long-enough-1234567890";

    public static Dictionary<string, string?> Values => new()
    {
        ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=Unused;Trusted_Connection=True",
        ["Jwt:Issuer"] = Issuer,
        ["Jwt:Audience"] = Audience,
        ["Jwt:SigningKey"] = SigningKey
    };
}

internal static class TestTokens
{
    public static string Create(Guid userId, string? role = "User", DateTime? expires = null,
        string? signingKey = null)
    {
        var claims = new List<Claim> { new("sub", userId.ToString()) };
        if (role is not null) claims.Add(new Claim("role", role));

        var token = new JwtSecurityToken(
            issuer: TestConfig.Issuer,
            audience: TestConfig.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow.AddMinutes(-10),
            expires: expires ?? DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey ?? TestConfig.SigningKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

internal sealed class TrainingWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:;Foreign Keys=False");

    public TrainingWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(TestConfig.Values));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<TrainingDbContext>>();
            services.RemoveAll<DbContextOptions>();

            // AddSqlServerConfiguration registers provider services in DI. Remove them so
            // only the SQLite provider remains in the WebApplicationFactory test host.
            var sqlServerServices = services
                .Where(d =>
                    d.ServiceType.Assembly.GetName().Name == "Microsoft.EntityFrameworkCore.SqlServer" ||
                    d.ImplementationType?.Assembly.GetName().Name == "Microsoft.EntityFrameworkCore.SqlServer")
                .ToList();

            foreach (var descriptor in sqlServerServices)
                services.Remove(descriptor);

            services.AddDbContext<TrainingDbContext>(options => options.UseSqlite(_connection));
        });
    }

    public TrainingDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<TrainingDbContext>().UseSqlite(_connection).Options);

    public void ResetDatabase()
    {
        using var db = CreateContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    public void Seed(params object[] entities)
    {
        using var db = CreateContext();
        db.Database.EnsureCreated();
        db.AddRange(entities);
        db.SaveChanges();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _connection.Dispose();
        base.Dispose(disposing);
    }
}
