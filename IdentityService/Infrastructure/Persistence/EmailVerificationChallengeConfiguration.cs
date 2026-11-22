using IdentityService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IdentityService.Infrastructure.Persistence;

public sealed class EmailVerificationChallengeConfiguration : IEntityTypeConfiguration<EmailVerificationChallenge>
{
    private static readonly ValueConverter<DateTime, DateTime> Utc =
        new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullable =
        new(v => v, v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

    public void Configure(EntityTypeBuilder<EmailVerificationChallenge> b)
    {
        b.ToTable("EmailVerificationChallenges");
        b.HasKey(c => c.Id);
        b.Property(c => c.Id).ValueGeneratedNever();
        b.Property(c => c.CodeHash).IsRequired().HasMaxLength(32); // HMAC-SHA256
        b.Property(c => c.CreatedAt).HasConversion(Utc);
        b.Property(c => c.ExpiresAt).HasConversion(Utc);
        b.Property(c => c.ConsumedAt).HasConversion(UtcNullable);
        b.Property(c => c.InvalidatedAt).HasConversion(UtcNullable);

        b.HasIndex(c => c.UserId).HasDatabaseName("IX_EmailVerificationChallenges_UserId");
        b.HasIndex(c => c.ExpiresAt).HasDatabaseName("IX_EmailVerificationChallenges_ExpiresAt");

        // At most one active (unused, not invalidated) challenge per user, enforced by the database.
        b.HasIndex(c => c.UserId)
            .IsUnique()
            .HasDatabaseName("UX_EmailVerificationChallenges_ActivePerUser")
            .HasFilter("ConsumedAt IS NULL AND InvalidatedAt IS NULL");
    }
}
