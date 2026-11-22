using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Controllers.ViewModels;
using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Orchestrators;

namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Controllers
{
    public static class CreateExerciseToWorkoutEndpoint
    {
        public static void MapCreateExerciseToWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/workouts/{workoutId:guid}/exercises", async (Guid workoutId, AddExerciseToWorkoutVM vm, IMediator mediator, CancellationToken ct) =>
            {
                var command = new CreateExerciseToWorkoutOrchestrator(
                    workoutId,
                    vm.ExerciseId,
                    vm.OrderIndex,
                    vm.TargetSets,
                    vm.TargetReps,
                    vm.TargetDurationSeconds
                );

                var result = await mediator.Send(command, ct);

                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("AddExerciseToWorkout");
        }
    }
}
