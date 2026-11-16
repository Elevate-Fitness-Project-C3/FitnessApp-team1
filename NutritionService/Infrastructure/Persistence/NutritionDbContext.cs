using System.Reflection;
using Microsoft.EntityFrameworkCore;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence;

public class NutritionDbContext : DbContext
{
    public NutritionDbContext(DbContextOptions<NutritionDbContext> options) : base(options)
    {
    }

    public DbSet<NutritionPreference> NutritionPreferences => Set<NutritionPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
