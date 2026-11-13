namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

//WorkoutSession
public class TrainingSession : BaseEntity
{
    public Guid UserId { get; set; }

    public int ProgramAttemptId { get; set; }
    public ProgramAttempt ProgramAttempt { get; set; } = null!;

    public int ProgramDayId { get; set; }
    public ProgramDay ProgramDay { get; set; } = null!;

    public SessionStatus Status { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public ICollection<PerformedExercise> PerformedExercises { get; set; } = new List<PerformedExercise>();
}
