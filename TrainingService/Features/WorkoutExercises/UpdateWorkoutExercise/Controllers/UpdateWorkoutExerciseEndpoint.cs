using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Commands;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Controllers.ViewModels;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Controllers
{
    public static class UpdateWorkoutExerciseEndpoint
    {
        public static void MapUpdateWorkoutExerciseEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/workouts/exercises/{id:guid}", async (Guid id, UpdateWorkoutExerciseVM vm, IMediator mediator, CancellationToken ct) =>
            {
                var command = new UpdateWorkoutExerciseCommand(
                    id,
                    vm.OrderIndex,
                    vm.TargetSets,
                    vm.TargetReps,
                    vm.TargetDurationSeconds
                );

                var result = await mediator.Send(command, ct);

                var response = EndpointResponse<RequestResponse<Unit>>.FromResult(RequestResponse<RequestResponse<Unit>>.Ok(result));
                return Results.Json(response, statusCode: response.StatusCode);
            })
                .WithTags("Workouts")
                .WithName("UpdateWorkoutExercise");
        }
    
    }
}
