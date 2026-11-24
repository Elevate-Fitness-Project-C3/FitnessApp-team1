using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Orchestrator;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Controllers.ViewModels;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Controllers
{
    public static class UpdateWorkoutExerciseEndpoint
    {
        public static void MapUpdateWorkoutExerciseEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/workouts/exercises/{id:guid}", async ([FromRoute] Guid id, UpdateWorkoutExerciseVM vm, IMediator mediator, CancellationToken ct) =>
            {
                var command = new UpdateWorkoutExerciseOrchestrator(
                    id,
                    vm.OrderIndex,
                    vm.TargetSets,
                    vm.TargetReps,
                    vm.TargetDurationSeconds
                );

                var result = await mediator.Send(command, ct);

                if (!result.Success)
                    return Results.Json(EndpointResponse<Unit>.Fail(result.Message, result.StatusCode), statusCode: result.StatusCode);

                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
                .WithTags("Workouts")
                .WithName("UpdateWorkoutExercise");
        }
    
    }
}
