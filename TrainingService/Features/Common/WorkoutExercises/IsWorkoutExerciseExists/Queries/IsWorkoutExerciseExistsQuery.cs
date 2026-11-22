using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseExists.Queries
{
    public record IsWorkoutExerciseExistsQuery(Guid WorkoutExerciseId) : IRequest<RequestResponse<bool>>;
}
