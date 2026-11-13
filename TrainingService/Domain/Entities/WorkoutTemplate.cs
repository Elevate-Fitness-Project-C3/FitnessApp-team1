namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

//Workout
public class WorkoutTemplate : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ExperienceLevel Difficulty { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public ICollection<WorkoutExercise> WorkoutExercises { get; set; } = new List<WorkoutExercise>();
    public ICollection<ProgramWorkoutSlot> ProgramWorkoutSlots { get; set; } = new List<ProgramWorkoutSlot>();
}
