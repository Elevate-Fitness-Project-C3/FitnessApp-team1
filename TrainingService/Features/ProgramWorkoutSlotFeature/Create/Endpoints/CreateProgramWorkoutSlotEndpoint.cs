using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using TrainingService.Features.ProgramWorkoutSlotFeature.Create.Commands;
using TrainingService.Features.ProgramWorkoutSlotFeature.Create.Orchestrator;
using TrainingService.Features.Shared;
using TrainingService.ViewModels.ProgramWorkoutSlot;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Create.Endpoints
{
    public static class CreateProgramWorkoutSlotEndpoint
    {
        public static void MapCreateProgramWorkoutSlotEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/programs/{programId:guid}/slots", async (
                Guid programId,
                [FromBody] CreateProgramWorkoutSlotVM body,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var request = new CreateProgramWorkoutSlotOrchestrator(
                    programId, body.WorkoutId, body.WeekNumber, body.DayNumber);

                var result = await mediator.Send(request, ct);
                var response = EndpointResponse<Guid>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("CreateProgramWorkoutSlot")
            .WithTags("Program Workout Slots")
            .RequireAuthorization("Admin");
            //.AllowAnonymous();

        }
    }
}
