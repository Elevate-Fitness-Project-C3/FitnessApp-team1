namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class Exercise : BaseEntity
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public MuscleGroup TargetMuscle { get; set; }
    public TrackingType TrackingType { get; set; }
    public bool IsActive { get; set; }

    public ICollection<PlannedExercise> PlannedExercises { get; set; } = new List<PlannedExercise>();
}
