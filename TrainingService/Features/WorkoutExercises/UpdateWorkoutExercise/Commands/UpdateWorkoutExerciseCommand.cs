using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Commands
{
    public record UpdateWorkoutExerciseCommand(    
        Guid WorkoutExerciseId,   // The ID of the workout exercise to update
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    ) : IRequest<RequestResponse<Unit>>;
}
