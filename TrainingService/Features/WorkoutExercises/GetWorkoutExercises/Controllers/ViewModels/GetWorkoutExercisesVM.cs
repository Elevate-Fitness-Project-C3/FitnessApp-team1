namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Controllers.ViewModels
{
    public record GetWorkoutExercisesVM(
        Guid WorkoutId,
        string WorkoutTitle,
        List<WorkoutExerciseItemVM> Exercises
    );
}
