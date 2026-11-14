namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class UserTrainingProgramConfiguration : IEntityTypeConfiguration<UserTrainingProgram>
{
    public void Configure(EntityTypeBuilder<UserTrainingProgram> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.TrainingProgram)
            .WithMany(p => p.UserTrainingPrograms)
            .HasForeignKey(x => x.TrainingProgramId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
