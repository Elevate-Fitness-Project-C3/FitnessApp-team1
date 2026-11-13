namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;


public class ProgramAttemptConfiguration : IEntityTypeConfiguration<ProgramAttempt>
{
    public void Configure(EntityTypeBuilder<ProgramAttempt> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingProgram)
            .WithMany(x => x.Attempts)
            .HasForeignKey(x => x.ProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
