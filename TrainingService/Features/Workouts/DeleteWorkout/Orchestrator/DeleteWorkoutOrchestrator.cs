using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.DeleteWorkout.Orchestrator
{
    public record DeleteWorkoutOrchestrator(Guid WorkoutId) : IRequest<RequestResponse<Unit>>;
}
