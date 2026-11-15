namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class SessionExercise : BaseEntity
{
    public Guid WorkoutSessionId { get; set; }
    public WorkoutSession WorkoutSession { get; set; } = null!;

    public Guid WorkoutExerciseId { get; set; }
    public WorkoutExercise WorkoutExercise { get; set; } = null!;

    public PerformedStatus Status { get; set; }
    public int? CompletedSets { get; set; }
    public int? CompletedReps { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? WeightKg { get; set; }
}
