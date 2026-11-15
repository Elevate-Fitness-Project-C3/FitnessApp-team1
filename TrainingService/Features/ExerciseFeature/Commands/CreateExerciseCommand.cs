using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Commands
{
    public record CreateExerciseCommand(string Name, string? Description, string TargetMuscle, string TrackingType, bool IsActive): IRequest<RequestResponse<Guid>>;
}
