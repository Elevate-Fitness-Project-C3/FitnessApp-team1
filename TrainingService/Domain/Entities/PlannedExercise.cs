namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;

//WorkoutExercise
public class PlannedExercise : BaseEntity
{
    public int WorkoutId { get; set; }
    public Workout Workout { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int OrderIndex { get; set; }
    public int? TargetSets { get; set; }
    public int? TargetReps { get; set; }
    public int? TargetDurationSeconds { get; set; }

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = new List<PerformedExercise>();
}
