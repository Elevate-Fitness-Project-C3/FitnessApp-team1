using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Controllers.ViewModels
{
    public record WorkoutItemVM(
        Guid Id,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
