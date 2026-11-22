using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Orchestrator
{
    public record DeleteWorkoutExerciseOrchestrator(Guid WorkoutExerciseId) : IRequest<RequestResponse<Unit>>;
}
