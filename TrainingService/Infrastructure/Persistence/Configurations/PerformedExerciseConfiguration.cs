namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class PerformedExerciseConfiguration : IEntityTypeConfiguration<PerformedExercise>
{
    public void Configure(EntityTypeBuilder<PerformedExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingSession)
            .WithMany(x => x.PerformedExercises)
            .HasForeignKey(x => x.TrainingSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.PlannedExercise)
            .WithMany(x => x.PerformedExercises)
            .HasForeignKey(x => x.PlannedExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
