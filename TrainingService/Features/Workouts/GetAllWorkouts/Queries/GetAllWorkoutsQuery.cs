using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetAllWorkouts.Dtos;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Queries
{
    public record GetAllWorkoutsQuery(int PageIndex = 1, int PageSize = 10)
        : IRequest<RequestResponse<PagedResult<WorkoutDto>>>;
}
