using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.CreateWorkout.Controllers.ViewModels
{
    public record CreateWorkoutVM(
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
