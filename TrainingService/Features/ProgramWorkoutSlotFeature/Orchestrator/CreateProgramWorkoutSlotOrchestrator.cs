using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Orchestrator
{
    public record CreateProgramWorkoutSlotOrchestrator(
        Guid ProgramId,
        Guid WorkoutId,
        int WeekNumber,
        int DayOfWeek) : IRequest<RequestResponse<Guid>>;
}
