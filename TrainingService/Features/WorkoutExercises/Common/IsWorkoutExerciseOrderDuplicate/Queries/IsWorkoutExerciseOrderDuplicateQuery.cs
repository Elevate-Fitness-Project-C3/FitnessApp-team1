using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Queries
{
    public record IsWorkoutExerciseOrderDuplicateQuery(
        int OrderIndex,
        Guid? WorkoutId = null,
        Guid? WorkoutExerciseId = null
    ) : IRequest<RequestResponse<bool>>;
}