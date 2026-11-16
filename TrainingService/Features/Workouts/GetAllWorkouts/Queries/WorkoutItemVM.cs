using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Queries
{
    public record WorkoutItemVM(
        Guid Id,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
