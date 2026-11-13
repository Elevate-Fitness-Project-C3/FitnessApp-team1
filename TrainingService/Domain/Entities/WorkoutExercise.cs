namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;

public class WorkoutExercise : BaseEntity
{
    public int WorkoutTemplateId { get; set; }
    public WorkoutTemplate WorkoutTemplate { get; set; } = null!;

    public int ExerciseId { get; set; }
    public Exercise Exercise { get; set; } = null!;

    public int OrderIndex { get; set; }
    public int? TargetSets { get; set; }
    public int? TargetReps { get; set; }
    public int? TargetDurationSeconds { get; set; }

    public ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
}
