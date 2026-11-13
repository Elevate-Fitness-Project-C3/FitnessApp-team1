namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class WorkoutSession : BaseEntity
{
    public Guid UserId { get; set; }

    public int UserTrainingProgramId { get; set; }
    public UserTrainingProgram UserTrainingProgram { get; set; } = null!;

    public int ProgramWorkoutSlotId { get; set; }
    public ProgramWorkoutSlot ProgramWorkoutSlot { get; set; } = null!;

    public SessionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<SessionExercise> SessionExercises { get; set; } = new List<SessionExercise>();
}
