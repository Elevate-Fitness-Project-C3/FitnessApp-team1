using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Controllers
{
    public static class PublishTrainingProgramEndpoint
    {
        public static void MapPublishTrainingProgramEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/training-programs/{id:guid}/publish",async ([FromRoute] Guid id, IMediator mediator, CancellationToken ct) =>
            {
                var result = await mediator.Send(new PublishTrainingProgramCommand(id), ct);
                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
        .WithTags("TrainingPrograms")
        .WithName("PublishTrainingProgram");
        }
    }
}
