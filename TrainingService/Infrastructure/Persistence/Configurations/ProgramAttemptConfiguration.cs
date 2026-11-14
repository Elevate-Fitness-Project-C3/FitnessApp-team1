namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;


public class ProgramAttemptConfiguration : IEntityTypeConfiguration<UserTrainingProgram>
{
    public void Configure(EntityTypeBuilder<UserTrainingProgram> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingProgram)
            .WithMany(x => x.UserTrainingPrograms)
            .HasForeignKey(x => x.TrainingProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
