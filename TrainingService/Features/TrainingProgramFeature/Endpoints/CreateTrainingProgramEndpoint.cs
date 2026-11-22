using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.DTOs;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Commands;
using TrainingService.Features.TrainingProgramFeature.Queries;

namespace TrainingService.Features.TrainingProgramFeature.Endpoints;



public static class CreateTrainingProgramEndpoint
{
    public static void MapCreateTrainingProgramEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/programs", async (
            [FromServices] IMediator mediator,
            [FromBody] CreateTrainingProgramCommand request,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(request, ct);
            var response = EndpointResponse<Guid>.FromResult(result);
            return Results.Json(response, statusCode: result.StatusCode);
        })
        .WithName("CreateTrainingProgram")
        .WithTags("Programs")
        .RequireAuthorization("AdminOnly");
    }
}