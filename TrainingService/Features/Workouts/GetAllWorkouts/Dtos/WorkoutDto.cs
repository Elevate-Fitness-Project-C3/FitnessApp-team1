using TrainingService.Domain.Enums;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Dtos
{
    public record WorkoutDto(
       Guid Id,
       string Title,
       string? Description,
       ExperienceLevel Difficulty,
       int EstimatedDurationMinutes
   );
}
