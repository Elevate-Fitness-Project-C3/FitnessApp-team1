using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Controllers;
using TrainingService.Features.Workouts.GetWorkoutExercises.Controllers;

namespace TrainingService.Features.WorkoutExercises
{
    public static class WorkoutExerciseEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapWorkoutExerciseEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapCreateExerciseToWorkoutEndpoint();
            app.MapGetWorkoutExercisesEndpoint();

            return app;
        }
    }
}
