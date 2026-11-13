namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;


//SessionExercise
public class PerformedExercise : BaseEntity
{
    public int TrainingSessionId { get; set; }
    public TrainingSession TrainingSession { get; set; } = null!;

    public int PlannedExerciseId { get; set; }
    public PlannedExercise PlannedExercise { get; set; } = null!;

    public PerformedStatus Status { get; set; }
    public int? CompletedSets { get; set; }
    public int? CompletedReps { get; set; }
    public int? DurationSeconds { get; set; }
    public decimal? WeightKg { get; set; }
}
