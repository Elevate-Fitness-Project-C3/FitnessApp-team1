using TrainingService.Features.Workouts.GetAllWorkouts.Dtos;
using TrainingService.Features.Workouts.GetAllWorkouts.Queries;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Controllers.ViewModels
{
    public record GetAllWorkoutsVM(
        List<WorkoutItemVM> Items,
        int PageNumber,
        int PageSize,
        int TotalCount,
        int TotalPages,
        bool HasPreviousPage,
        bool HasNextPage
    );
}
