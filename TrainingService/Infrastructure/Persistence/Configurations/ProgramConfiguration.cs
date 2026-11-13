namespace TrainingService.Infrastructure.Persistence.Configurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrainingService.Domain.Entities;

public class ProgramConfiguration : IEntityTypeConfiguration<TrainingProgram>
{
    public void Configure(EntityTypeBuilder<TrainingProgram> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
