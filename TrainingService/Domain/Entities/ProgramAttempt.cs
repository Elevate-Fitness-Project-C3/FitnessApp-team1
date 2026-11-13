namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

//UserTrainingProgram
public class ProgramAttempt : BaseEntity
{
    public Guid UserId { get; set; }

    public int ProgramId { get; set; }
    public TrainingProgram TrainingProgram { get; set; } = null!;

    public AttemptStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? StoppedAt { get; set; }

    public ICollection<TrainingSession> Sessions { get; set; } = new List<TrainingSession>();
}
