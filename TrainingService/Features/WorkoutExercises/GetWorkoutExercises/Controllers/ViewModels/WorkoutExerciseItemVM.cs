namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Controllers.ViewModels
{
    public record WorkoutExerciseItemVM(
        Guid WorkoutExerciseId,
        Guid ExerciseId,
        string ExerciseName,
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    );
}
