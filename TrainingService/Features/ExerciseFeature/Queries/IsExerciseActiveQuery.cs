using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Queries
{
    public record IsExerciseActiveQuery(Guid ExerciseId) : IRequest<RequestResponse<bool>>;
}
