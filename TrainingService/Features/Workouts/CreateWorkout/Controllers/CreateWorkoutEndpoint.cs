using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.CreateWorkout.Commands;
using TrainingService.Features.Workouts.CreateWorkout.Controllers.ViewModels;

namespace TrainingService.Features.Workouts.CreateWorkout.Controllers
{
    public static class CreateWorkoutEndpoint
    {
        public static void MapCreateWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/workouts", async (CreateWorkoutVM request, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = new CreateWorkoutCommand(
                    request.Title,
                    request.Description,
                    request.Difficulty,
                    request.EstimatedDurationMinutes
                );

                var result = await mediator.Send(command, cancellationToken);
                var response = EndpointResponse<RequestResponse<Unit>>.FromResult(RequestResponse<RequestResponse<Unit>>.Ok(result));
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("CreateWorkout");
        }
    }
}
