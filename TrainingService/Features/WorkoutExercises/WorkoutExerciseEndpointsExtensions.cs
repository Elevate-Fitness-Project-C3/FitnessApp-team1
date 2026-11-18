using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Controllers;

namespace TrainingService.Features.WorkoutExercises
{
    public static class WorkoutExerciseEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapWorkoutExerciseEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapAddExerciseToWorkoutEndpoint();

            return app;
        }
    }
}
