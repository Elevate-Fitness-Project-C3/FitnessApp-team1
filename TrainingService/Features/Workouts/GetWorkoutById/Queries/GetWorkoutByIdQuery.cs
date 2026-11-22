using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetWorkoutById.Dtos;

namespace TrainingService.Features.Workouts.GetWorkoutById.Queries
{
    public record GetWorkoutByIdQuery(Guid WorkoutId) : IRequest<RequestResponse<WorkoutByIdDto>>;
}
