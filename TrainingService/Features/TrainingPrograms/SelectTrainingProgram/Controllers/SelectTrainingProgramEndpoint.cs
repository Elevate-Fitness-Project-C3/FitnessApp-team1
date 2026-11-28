using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Orchestrator;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Controllers
{
    public static class SelectTrainingProgramEndpoint
    {
        public static void MapSelectTrainingProgramEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/training-programs/{id:guid}/select", async (
               [FromRoute] Guid id,
                bool AcceptWarnings,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new SelectTrainingProgramOrchestrator(id, AcceptWarnings), ct);
                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("TrainingPrograms")
            .WithName("SelectTrainingProgram")
            .RequireAuthorization();
        }
    }
}
