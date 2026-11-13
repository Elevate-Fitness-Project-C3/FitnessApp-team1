namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.ProgramAttempt)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.ProgramAttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ProgramDay)
            .WithMany(x => x.Sessions)
            .HasForeignKey(x => x.ProgramDayId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
