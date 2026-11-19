using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Exercises.IsExerciseExists.Queries
{
    public record IsExerciseExistsQuery(Guid ExerciseId) : IRequest<RequestResponse<bool>>;

}
