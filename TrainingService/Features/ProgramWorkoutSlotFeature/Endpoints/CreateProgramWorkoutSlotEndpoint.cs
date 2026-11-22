using MediatR;
using TrainingService.Features.ProgramWorkoutSlotFeature.Orchestrator;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints
{
    public static class CreateProgramWorkoutSlotEndpoint
    {
        public static void MapCreateProgramWorkoutSlotEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/program-workout-slots", async ([AsParameters] CreateProgramWorkoutSlotOrchestrator request, IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                var response = EndpointResponse<Guid>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("CreateProgramWorkoutSlot")
            .WithTags("Program Workout Slots");
            //.RequireAuthorization("AdminPolicy"); 
        }
    }
}
