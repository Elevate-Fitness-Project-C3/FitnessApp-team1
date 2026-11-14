namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class PerformedExerciseConfiguration : IEntityTypeConfiguration<SessionExercise>
{
    public void Configure(EntityTypeBuilder<SessionExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkoutSession)
            .WithMany(x => x.SessionExercises)
            .HasForeignKey(x => x.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkoutExercise)
            .WithMany(x => x.SessionExercises)
            .HasForeignKey(x => x.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
