namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class ProgramWorkoutSlotConfiguration : IEntityTypeConfiguration<ProgramWorkoutSlot>
{
    public void Configure(EntityTypeBuilder<ProgramWorkoutSlot> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingProgram)
            .WithMany(p => p.ProgramWorkoutSlots)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WorkoutTemplate)
            .WithMany(w => w.ProgramWorkoutSlots)
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.ProgramId, x.WeekNumber, x.DayNumber })
            .IsUnique();
    }
}
