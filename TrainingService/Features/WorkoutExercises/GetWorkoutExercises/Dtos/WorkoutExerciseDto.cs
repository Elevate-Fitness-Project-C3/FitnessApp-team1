namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Dtos
{
    public record WorkoutExerciseDto(
        Guid WorkoutExerciseId,
        Guid ExerciseId,
        string ExerciseName,
        int OrderIndex,
        int? TargetSets,
        int? TargetReps,
        int? TargetDurationSeconds
    );
}
