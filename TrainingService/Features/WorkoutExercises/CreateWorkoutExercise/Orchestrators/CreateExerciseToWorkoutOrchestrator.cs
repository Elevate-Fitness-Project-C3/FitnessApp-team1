using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Orchestrators
{
    public record CreateExerciseToWorkoutOrchestrator(
        Guid WorkoutId,
        Guid ExerciseId,
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    ) : IRequest<RequestResponse<Unit>>;
}
