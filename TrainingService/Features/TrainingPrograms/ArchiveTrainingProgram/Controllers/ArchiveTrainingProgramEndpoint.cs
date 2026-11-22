using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Controllers
{
    public static class ArchiveTrainingProgramEndpoint
    {
        public static void MapArchiveTrainingProgramEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/training-programs/{id:guid}/archive", async ([FromRoute] Guid id, IMediator mediator, CancellationToken ct) =>
                {
                    var result = await mediator.Send(new ArchiveTrainingProgramCommand(id), ct);
                    var response = EndpointResponse<Unit>.FromResult(result);

                    return Results.Json(response, statusCode: response.StatusCode);
                })
            .WithTags("TrainingPrograms")
            .WithName("ArchiveTrainingProgram");
        }
    }
}
