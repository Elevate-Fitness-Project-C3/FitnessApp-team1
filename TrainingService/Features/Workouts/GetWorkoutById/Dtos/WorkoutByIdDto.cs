using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.GetWorkoutById.Dtos
{
    public record WorkoutByIdDto(
        Guid Id,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    );
}
