using FitnessProfileService.Domain;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Infrastructure.Persistence;

public class FitnessProfileDbContext(DbContextOptions<FitnessProfileDbContext> options) : DbContext(options)
{
    public DbSet<FitnessProfile> FitnessProfiles => Set<FitnessProfile>();
    public DbSet<BodyMeasurement> BodyMeasurements => Set<BodyMeasurement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
        => modelBuilder.ApplyConfigurationsFromAssembly(typeof(FitnessProfileDbContext).Assembly);
}
