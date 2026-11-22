using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.Entities;

namespace ProgressService.Infrastrucure.Persistence.Configurations
{
    public class ProgressDbContext(DbContextOptions<ProgressDbContext> options) : DbContext(options)
    {
        public DbSet<CompletedWorkoutRecord> CompletedWorkoutRecords => Set<CompletedWorkoutRecord>();
        public DbSet<UserStatistics> UserStatistics => Set<UserStatistics>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            var configurations = new ProgressConfigurations();

            modelBuilder.ApplyConfiguration<CompletedWorkoutRecord>(configurations);
            modelBuilder.ApplyConfiguration<UserStatistics>(configurations);
            modelBuilder.ApplyConfiguration<Achievement>(configurations);
            modelBuilder.ApplyConfiguration<UserAchievement>(configurations);
        }
    }
}
