using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.UpdateWorkout.Controllers.ViewModels
{
    public record UpdateWorkoutVM(
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
