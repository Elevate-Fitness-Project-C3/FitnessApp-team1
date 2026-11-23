namespace TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Dtos
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
