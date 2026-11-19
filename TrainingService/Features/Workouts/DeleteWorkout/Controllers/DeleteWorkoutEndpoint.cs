using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Orchestrator;

namespace TrainingService.Features.Workouts.DeleteWorkout.Controllers
{
    public static class DeleteWorkoutEndpoint
    {
        public static void MapDeleteWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/workouts/{workoutId:guid}", async ([FromRoute] Guid workoutId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new DeleteWorkoutOrchestrator(workoutId), cancellationToken);
                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("DeleteWorkout");
        } 
    }
}
