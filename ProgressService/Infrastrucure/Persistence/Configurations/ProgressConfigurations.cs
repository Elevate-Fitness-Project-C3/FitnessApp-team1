using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgressService.Domain.Entities;
namespace ProgressService.Infrastrucure.Persistence.Configurations
{
    public class ProgressConfigurations :
    IEntityTypeConfiguration<CompletedWorkoutRecord>,
    IEntityTypeConfiguration<UserStatistics>,
    IEntityTypeConfiguration<Achievement>,
    IEntityTypeConfiguration<UserAchievement>
    {
        public void Configure(EntityTypeBuilder<CompletedWorkoutRecord> builder)
        {
            builder.ToTable("CompletedWorkoutRecords");
            builder.HasKey(x=> x.Id);

            //PG-01 : 
            builder.HasIndex(x => x.WorkoutSessionId).IsUnique();
            //PG-02 : 
            builder.HasIndex(x => new { x.UserId, x.CompletedAtUtc, x.Id })
                   .IsDescending(false, true, true);
        }
        public void Configure(EntityTypeBuilder<UserStatistics> builder)
        {
            builder.ToTable("UserStatistics", t => t.HasCheckConstraint(
            "CK_UserStatistics_NonNegative",
            "[TotalCompletedWorkouts] >= 0 AND [CurrentStreak] >= 0 AND [LongestStreak] >= 0"));

            builder.HasKey(x => x.UserId);                  
            builder.Property(x => x.UserId).ValueGeneratedNever();
        }

        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.ToTable("Achievements");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired().HasMaxLength(500);
            builder.Property(x => x.RuleCode).IsRequired().HasMaxLength(50);

            builder.HasIndex(x => x.RuleCode).IsUnique();  
        }

        public void Configure(EntityTypeBuilder<UserAchievement> builder)
        {
            builder.ToTable("UserAchievements");
            builder.HasKey(x => x.Id);

            // Progress: Achievement (1) ← UserAchievement (N)
            builder.HasOne(x => x.Achievement)
             .WithMany(a => a.UserAchievements)
             .HasForeignKey(x => x.AchievementId)
             .OnDelete(DeleteBehavior.Restrict);     

            // PG-04:
            builder.HasIndex(x => new { x.UserId, x.AchievementId }).IsUnique();
        }

    }
}
