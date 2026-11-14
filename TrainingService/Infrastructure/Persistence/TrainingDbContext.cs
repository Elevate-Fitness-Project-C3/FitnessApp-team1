namespace TrainingService.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;

public class TrainingDbContext(DbContextOptions<TrainingDbContext> options) : DbContext(options)
{
    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
    public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
    public DbSet<TrainingProgram> TrainingPrograms { get; set; }
    public DbSet<ProgramWorkoutSlot> ProgramWorkoutSlots { get; set; }
    public DbSet<UserTrainingProgram> UserTrainingPrograms { get; set; }
    public DbSet<WorkoutSession> WorkoutSessions { get; set; }
    public DbSet<SessionExercise> SessionExercises { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrainingDbContext).Assembly);
    }
}
