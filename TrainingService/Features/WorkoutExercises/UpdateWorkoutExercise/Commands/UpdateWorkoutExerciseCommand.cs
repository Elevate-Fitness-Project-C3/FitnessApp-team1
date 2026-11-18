using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Commands
{
    public record UpdateWorkoutExerciseCommand(
        Guid WorkoutExerciseId, 
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    ) : IRequest<RequestResponse<Unit>>;
}
