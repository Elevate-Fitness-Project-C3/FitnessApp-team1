using FitnessProfileService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FitnessProfileService.Infrastructure.Persistence.Configurations;

public sealed class BodyMeasurementConfiguration : IEntityTypeConfiguration<BodyMeasurement>
{
    public void Configure(EntityTypeBuilder<BodyMeasurement> b)
    {
        b.ToTable("BodyMeasurement");

        b.HasKey(m => m.MeasurementId);
        b.Property(m => m.MeasurementId).ValueGeneratedOnAdd();

        b.Property(m => m.Id).ValueGeneratedNever();
        b.Property(m => m.UserId).IsRequired();

        b.Property(m => m.Height).HasPrecision(5, 1).IsRequired();
        b.Property(m => m.Weight).HasPrecision(5, 1).IsRequired();

        b.Property(m => m.CreatedAt).HasConversion(UtcConverters.Required);
        b.Property(m => m.UpdatedAt).HasConversion(UtcConverters.Optional);
        b.Property(m => m.DeletedAt).HasConversion(UtcConverters.Optional);

        b.HasIndex(m => new { m.UserId, m.CreatedAt, m.MeasurementId })
            .IsDescending(false, true, true)
            .HasDatabaseName("IX_BodyMeasurement_UserId_RecordedAt_MeasurementId");
    }
}
