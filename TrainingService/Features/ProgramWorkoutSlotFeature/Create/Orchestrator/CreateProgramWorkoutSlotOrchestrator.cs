using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Create.Orchestrator
{
    public record CreateProgramWorkoutSlotOrchestrator(
        Guid ProgramId,
        Guid WorkoutId,
        int WeekNumber,
        int DayNumber) : IRequest<RequestResponse<Guid>>;
}
