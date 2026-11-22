namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Controllers.ViewModels
{
    public record UpdateWorkoutExerciseVM(
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    );
}
