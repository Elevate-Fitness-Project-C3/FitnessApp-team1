using FitnessProfileService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessProfileService.Infrastructure.Persistence.Configurations;

public sealed class FitnessProfileConfiguration : IEntityTypeConfiguration<FitnessProfile>
{
    public void Configure(EntityTypeBuilder<FitnessProfile> b)
    {
        b.ToTable("FitnessProfile", t => t.HasCheckConstraint(
            "CK_FitnessProfile_AvailableDaysPerWeek",
            "AvailableDaysPerWeek IS NULL OR AvailableDaysPerWeek BETWEEN 1 AND 7"));

        b.HasKey(p => p.Id);
        b.Property(p => p.Id).ValueGeneratedNever();

        b.HasIndex(p => p.UserId).IsUnique().HasDatabaseName("IX_FitnessProfile_UserId");

        b.Property(p => p.UserId).IsRequired();
        b.Property(p => p.DateOfBirth).HasColumnType("date").IsRequired();

        b.Property(p => p.Gender).HasConversion<string>().HasMaxLength(10).IsRequired();
        b.Property(p => p.PrimaryFitnessGoal).HasConversion<string>().HasMaxLength(30).IsRequired();
        b.Property(p => p.PhysicalActivityLevel).HasConversion<string>().HasMaxLength(30).IsRequired();

        b.Property(p => p.TrainingExperienceLevel).HasMaxLength(30);

        b.Property(p => p.IsOnboardingCompleted).IsRequired();

        b.Property(p => p.CreatedAt).HasConversion(UtcConverters.Required);
        b.Property(p => p.UpdatedAt).HasConversion(UtcConverters.Optional);
        b.Property(p => p.DeletedAt).HasConversion(UtcConverters.Optional);
    }
}
