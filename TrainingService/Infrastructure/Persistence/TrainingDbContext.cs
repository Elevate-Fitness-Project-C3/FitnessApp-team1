namespace TrainingService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;

public class TrainingDbContext(DbContextOptions<TrainingDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutTemplate> Workouts { get; set; }
    public DbSet<WorkoutExercise> PlannedExercises { get; set; }
    public DbSet<TrainingProgram> Programs { get; set; }
    public DbSet<ProgramWorkoutSlot> ProgramDays { get; set; }
    public DbSet<UserTrainingProgram> ProgramAttempts { get; set; }
    public DbSet<WorkoutSession> TrainingSessions { get; set; }
    public DbSet<SessionExercise> PerformedExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
    }
}
