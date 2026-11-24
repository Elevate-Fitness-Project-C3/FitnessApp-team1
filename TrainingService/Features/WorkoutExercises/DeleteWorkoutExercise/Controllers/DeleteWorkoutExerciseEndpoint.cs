using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Orchestrator;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Controllers
{
    public static class DeleteWorkoutExerciseEndpoint
    {
        public static void MapDeleteWorkoutExerciseEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/workouts/exercises/{id:guid}", async ([FromRoute] Guid id, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new DeleteWorkoutExerciseOrchestrator(id), ct);

                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
                .WithTags("Workouts")
                .WithName("DeleteWorkoutExercise");
        }
    }
}
