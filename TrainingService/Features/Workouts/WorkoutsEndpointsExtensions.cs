using TrainingService.Features.Workouts.CreateWorkout.Controllers;
using TrainingService.Features.Workouts.DeleteWorkout.Controllers;
using TrainingService.Features.Workouts.GetAllWorkouts.Controllers;
using TrainingService.Features.Workouts.UpdateWorkout.Controllers;

namespace TrainingService.Features.Workouts
{
    public static class WorkoutsEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapWorkoutsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapCreateWorkoutEndpoint();      
            //app.MapGetWorkout();      
            app.MapGetAllWorkoutsEndpoint();        
            app.MapUpdateWorkoutEndpoint();    
            app.MapDeleteWorkoutEndpoint();      

            return app;
        }
    }
}
