namespace TrainingService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;

public class TrainingDbContext(DbContextOptions<TrainingDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<Workout> Workouts { get; set; }
    public DbSet<PlannedExercise> PlannedExercises { get; set; }
    public DbSet<TrainingProgram> Programs { get; set; }
    public DbSet<ProgramDay> ProgramDays { get; set; }
    public DbSet<ProgramAttempt> ProgramAttempts { get; set; }
    public DbSet<TrainingSession> TrainingSessions { get; set; }
    public DbSet<PerformedExercise> PerformedExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
    }
}
