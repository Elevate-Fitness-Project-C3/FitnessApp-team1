namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class SessionExerciseConfiguration : IEntityTypeConfiguration<SessionExercise>
{
    public void Configure(EntityTypeBuilder<SessionExercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.WorkoutSession)
            .WithMany(s => s.SessionExercises)
            .HasForeignKey(x => x.WorkoutSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkoutExercise)
            .WithMany(e => e.SessionExercises)
            .HasForeignKey(x => x.WorkoutExerciseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
