using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence.Configurations;

public class NutritionPreferenceConfig : IEntityTypeConfiguration<NutritionPreference>
{
    public void Configure(EntityTypeBuilder<NutritionPreference> builder)
    {
        builder.ToTable("NutritionPreferences");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.UserId)
            .IsUnique();

        builder.Property(x => x.Goal)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();
    }
}
