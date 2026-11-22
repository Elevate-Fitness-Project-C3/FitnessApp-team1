using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutTemplateFeature.Queries
{
    public record WorkoutExistsQuery(Guid WorkoutId) : IRequest<RequestResponse<bool>>;
}
