namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class ProgramDayConfiguration : IEntityTypeConfiguration<ProgramDay>
{
    public void Configure(EntityTypeBuilder<ProgramDay> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingProgram)
            .WithMany(x => x.Days)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Workout)
            .WithMany(x => x.ProgramDays)
            .HasForeignKey(x => x.WorkoutId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
