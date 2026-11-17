using MediatR;

namespace ProgressService.Features.completedworkoutRecordFeature.Commands
{
    public record RecordCompletedWorkoutCommand(
        Guid UserId,
        Guid WorkoutSessionId,
        Guid TrainingProgramId,
        Guid ProgramWorkoutSlotId,
        DateTime CompletedAtUtc): IRequest<bool>;
}
