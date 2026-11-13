namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;

//ProgramWorkout
public class ProgramDay : BaseEntity
{
    public int ProgramId { get; set; }
    public TrainingProgram TrainingProgram { get; set; } = null!;

    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int WeekNumber { get; set; }
    public int DayNumber { get; set; }

    public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
}
