using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Queries
{
    public record ExerciseExistsQuery(Guid ExerciseId) : IRequest<RequestResponse<bool>>;
}
