using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NutritionService.Domain.Entities;

namespace NutritionService.Infrastructure.Persistence.Configurations;

public class MealConfig : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        builder.ToTable("Meals", t =>
        {
            t.HasCheckConstraint("CK_Meals_Calories_NonNegative", "[Calories] >= 0");
            t.HasCheckConstraint("CK_Meals_Protein_NonNegative", "[ProteinGrams] >= 0");
        });

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Calories)
            .IsRequired();

        builder.Property(x => x.ProteinGrams)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired(false);

        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Name);
    }
}
