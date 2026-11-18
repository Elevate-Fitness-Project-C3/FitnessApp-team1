using Microsoft.EntityFrameworkCore;

namespace IdentityService.Features.EmailVerification;

public sealed class EmailVerificationDbContext(DbContextOptions<EmailVerificationDbContext> options) : DbContext(options)
{
    public DbSet<EmailVerificationChallenge> Challenges => Set<EmailVerificationChallenge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailVerificationChallenge>(e =>
        {
            e.ToTable("EmailVerificationChallenges");
            e.HasKey(c => c.Id);
            e.Property(c => c.CodeHash).IsRequired().HasMaxLength(32);

            e.HasIndex(c => c.UserId).HasDatabaseName("IX_EmailVerificationChallenges_UserId");
            e.HasIndex(c => c.ExpiresAt).HasDatabaseName("IX_EmailVerificationChallenges_ExpiresAt");

            // Only ONE active (unused + not invalidated) challenge per user, enforced by the database.
            e.HasIndex(c => c.UserId)
                .IsUnique()
                .HasFilter("[ConsumedAt] IS NULL AND [InvalidatedAt] IS NULL")
                .HasDatabaseName("UX_EmailVerificationChallenges_ActivePerUser");
        });
    }
}
