using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.UpdateWorkout.Orchestrator;
using TrainingService.Features.Workouts.UpdateWorkout.Controllers.ViewModels;

namespace TrainingService.Features.Workouts.UpdateWorkout.Controllers
{
    public static class UpdateWorkoutEndpoint
    {
        public static void MapUpdateWorkoutEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPut("/api/workouts/{id}", async (Guid id, UpdateWorkoutVM vm, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var command = new UpdateWorkoutOrchestrator(
                    id,
                    vm.Title,
                    vm.Description,
                    vm.Difficulty,
                    vm.EstimatedDurationMinutes
                );

                var result = await mediator.Send(command, cancellationToken);
                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("UpdateWorkout");
        }
    }
}
