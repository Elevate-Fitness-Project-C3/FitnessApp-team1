using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Queries
{
    public record IsExerciseNameTakenQuery(string Name) : IRequest<RequestResponse<bool>>;
}
