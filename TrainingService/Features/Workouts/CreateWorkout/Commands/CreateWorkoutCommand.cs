using MediatR;
using TrainingService.Domain.Enums;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Workouts.CreateWorkout.Commands
{
    public record CreateWorkoutCommand(
        string Title,
        string? Description,
        ExperienceLevel Difficulty,
        int EstimatedDurationMinutes
    ) : IRequest<RequestResponse>;
}
