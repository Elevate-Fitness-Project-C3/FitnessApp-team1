using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.GetWorkoutById.Controllers.ViewModels
{
    public record GetWorkoutByIdVM(
        Guid Id,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
