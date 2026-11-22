namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Dtos
{
    public record WorkoutWithExercisesDto(
        Guid WorkoutId,
        string WorkoutTitle,
        List<WorkoutExerciseDto> Exercises
    );
}
