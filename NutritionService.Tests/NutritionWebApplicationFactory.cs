using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Infrastructure.Persistence;

namespace NutritionService.Tests;

public class TestFitnessProfileClient : IFitnessProfileClient
{
    public FitnessProfileResult Result { get; set; } = FitnessProfileResult.Success(2200);

    public Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Result);
}

public class NutritionWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:;Foreign Keys=False");
    public TestFitnessProfileClient FitnessProfileClientStub { get; } = new();

    public NutritionWebApplicationFactory()
    {
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(TestConfig.Values));
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<NutritionDbContext>(options => options.UseSqlite(_connection));

            // Replace IFitnessProfileClient with test stub
            services.RemoveAll<IFitnessProfileClient>();
            services.AddSingleton<IFitnessProfileClient>(FitnessProfileClientStub);
        });
    }

    public NutritionDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<NutritionDbContext>().UseSqlite(_connection).Options);

    public void Seed(params object[] entities)
    {
        using var db = CreateContext();
        db.Database.EnsureCreated();
        db.AddRange(entities);
        db.SaveChanges();
    }

    public void ResetDatabase()
    {
        using var db = CreateContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _connection.Dispose();
        base.Dispose(disposing);
    }
}
