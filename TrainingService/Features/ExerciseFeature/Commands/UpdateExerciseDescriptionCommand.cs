using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Commands
{
    public record UpdateExerciseDescriptionCommand(Guid ExerciseId, string Description) : IRequest<RequestResponse<Unit>>; 
}
