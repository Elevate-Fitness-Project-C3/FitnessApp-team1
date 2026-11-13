namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class UserTrainingProgram : BaseEntity
{
    public Guid UserId { get; set; }

    public int TrainingProgramId { get; set; }
    public TrainingProgram TrainingProgram { get; set; } = null!;

    public AttemptStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? StoppedAt { get; set; }

    public ICollection<WorkoutSession> WorkoutSessions { get; set; } = new List<WorkoutSession>();
}
