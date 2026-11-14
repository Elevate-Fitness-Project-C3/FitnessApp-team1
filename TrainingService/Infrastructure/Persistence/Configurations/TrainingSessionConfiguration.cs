namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<WorkoutSession>
{
    public void Configure(EntityTypeBuilder<WorkoutSession> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.UserTrainingProgram)
            .WithMany(x => x.WorkoutSessions)
            .HasForeignKey(x => x.UserTrainingProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProgramWorkoutSlot)
            .WithMany(x => x.WorkoutSessions)
            .HasForeignKey(x => x.ProgramWorkoutSlotId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
