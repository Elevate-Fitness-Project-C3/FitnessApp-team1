using MediatR;
using TrainingService.Domain.Enums;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.UpdateWorkout.Orchestrator
{
    public record UpdateWorkoutOrchestrator(
        Guid WorkoutId,
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    ) : IRequest<RequestResponse<Unit>>;
}
