using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Enums;
using NutritionService.Infrastructure.Persistence;
using NutritionService.Infrastructure.Persistence.Repositories;

namespace NutritionService.Tests;

public static class TestConfig
{
    public const string Issuer = "fitnessapp-team1";
    public const string Audience = "fitnessapp-team1-clients";
    public const string SigningKey = "test-signing-key-that-is-definitely-long-enough-1234567890";

    public static Dictionary<string, string?> Values => new()
    {
        ["ConnectionStrings:Elevate_NutritionDb"] = "Server=localhost;Database=Unused;Trusted_Connection=True",
        ["Jwt:Issuer"] = Issuer,
        ["Jwt:Audience"] = Audience,
        ["Jwt:SigningKey"] = SigningKey,
        ["FitnessProfile:BaseUrl"] = "http://localhost:5002",
        ["FitnessProfile:TdeeEndpoint"] = "/api/profile/me/tdee",
        ["FitnessProfile:TimeoutSeconds"] = "10"
    };
}

public static class TestTokens
{
    public static string Create(Guid userId, string? role = "User", DateTime? expires = null, string? signingKey = null)
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

public class TestCurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; set; } = Guid.NewGuid();
    public bool IsAdmin { get; set; }

    public Guid GetRequiredUserId() => UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
}

public class TestDatabaseContext : IDisposable
{
    public SqliteConnection Connection { get; }
    public NutritionDbContext DbContext { get; }
    public IGenericRepository<NutritionPreference> PreferenceRepository { get; }

    public TestDatabaseContext()
    {
        Connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
        Connection.Open();

        var options = new DbContextOptionsBuilder<NutritionDbContext>()
            .UseSqlite(Connection)
            .Options;

        DbContext = new NutritionDbContext(options);
        DbContext.Database.EnsureCreated();

        PreferenceRepository = new GenericRepository<NutritionPreference>(DbContext);
    }

    public void SeedPreference(Guid userId, NutritionGoal goal)
    {
        var pref = NutritionPreference.Create(userId, goal, DateTime.UtcNow);
        DbContext.NutritionPreferences.Add(pref);
        DbContext.SaveChanges();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        Connection.Dispose();
    }
}

public static class TestLoggerFactory
{
    public static ILogger<T> CreateLogger<T>() => LoggerFactory.Create(builder => builder.AddFilter(_ => false)).CreateLogger<T>();
}
