using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.DeleteWorkout.Queries
{
    public record IsWorkoutExistsQuery(Guid WorkoutId) : IRequest<RequestResponse<bool>>;

}
