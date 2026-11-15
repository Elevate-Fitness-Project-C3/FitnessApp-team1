using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.CreateWorkout.Commands;

namespace TrainingService.Features.Workouts.CreateWorkout.Controllers
{
    public static class CreateWorkoutEndpoint
    {
        public static void MapCreateWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/workouts", async (CreateWorkoutRequest request, IMediator mediator) =>
            {
                // 1. تحويل الـ View Model إلى Command
                var command = new CreateWorkoutCommand(
                    request.Title,
                    request.Description,
                    request.Difficulty,
                    request.EstimatedDurationMinutes
                );

                var result = await mediator.Send(command);

                if (!result.Success)
                    return Results.Json(EndpointResponse.Fail(result.Message, result.StatusCode, result.Errors), statusCode: result.StatusCode);

                return Results.Json(EndpointResponse.Ok(result.Message, result.StatusCode), statusCode: result.StatusCode);
            })
                  .WithTags("Workouts")
                  .WithName("CreateWorkout");
        }
    }
}
