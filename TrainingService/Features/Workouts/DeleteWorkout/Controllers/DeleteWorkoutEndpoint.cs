using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Commands;

namespace TrainingService.Features.Workouts.DeleteWorkout.Controllers
{
    public static class DeleteWorkoutEndpoint
    {
        public static void MapDeleteWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/workouts/{workoutId}", async (Guid workoutId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = new DeleteWorkoutCommand(workoutId);
                var result = await mediator.Send(command, cancellationToken);
                var response = EndpointResponse<RequestResponse<Unit>>.FromResult(RequestResponse<RequestResponse<Unit>>.Ok(result));
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("DeleteWorkout");
        } 
    }
}
