using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Infrastructure.Persistence;

/// <summary>
/// Standalone so ID-02 doesn't depend on ID-01. To merge into your existing DbContext later:
/// add the DbSet and call modelBuilder.ApplyConfiguration(new EmailVerificationChallengeConfiguration()).
/// </summary>
public class EmailVerificationDbContext(DbContextOptions<EmailVerificationDbContext> options) : DbContext(options)
{
    public DbSet<EmailVerificationChallenge> Challenges => Set<EmailVerificationChallenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new EmailVerificationChallengeConfiguration());
}
