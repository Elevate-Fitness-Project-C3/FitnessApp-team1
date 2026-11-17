using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Commands
{
    public record DeactivateExerciseCommand(Guid ExerciseId):IRequest<RequestResponse<Unit>>;
}
