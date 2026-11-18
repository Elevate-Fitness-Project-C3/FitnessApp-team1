using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries
{
    public record IsWorkoutExistsQuery(Guid WorkoutId) : IRequest<RequestResponse<bool>>;

}
