namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Controllers.ViewModels
{
    public record AddExerciseToWorkoutVM(
        Guid ExerciseId,
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    );
}
