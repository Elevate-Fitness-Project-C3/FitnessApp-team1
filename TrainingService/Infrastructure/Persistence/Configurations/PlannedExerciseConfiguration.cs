namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class PlannedExerciseConfiguration : IEntityTypeConfiguration<PlannedExercise>
{
    public void Configure(EntityTypeBuilder<PlannedExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Workout)
            .WithMany(x => x.PlannedExercises)
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Exercise)
            .WithMany(x => x.PlannedExercises)
            .HasForeignKey(x => x.ExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
