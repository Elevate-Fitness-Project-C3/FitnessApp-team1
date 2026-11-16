using MediatR;
using TrainingService.Domain.Enums;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.UpdateWorkout.Commands
{
    public record UpdateWorkoutCommand(
        Guid WorkoutId,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    ) : IRequest<RequestResponse<Unit>>;
}
