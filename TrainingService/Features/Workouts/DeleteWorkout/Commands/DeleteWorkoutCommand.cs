using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.DeleteWorkout.Commands
{
    public record DeleteWorkoutCommand(Guid WorkoutId) : IRequest<RequestResponse<Unit>>;
}
