namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;

//ProgramWorkout ==> One day in a program
public class ProgramWorkoutSlot : BaseEntity
{
    public Guid ProgramId { get; set; }
    public TrainingProgram TrainingProgram { get; set; } = null!;

    public Guid WorkoutId { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;

    public int WeekNumber { get; set; }
    public int DayNumber { get; set; }

    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
