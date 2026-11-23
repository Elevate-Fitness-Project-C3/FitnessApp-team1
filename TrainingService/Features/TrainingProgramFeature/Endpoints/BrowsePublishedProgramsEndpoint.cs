using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Queries;
using TrainingService.Mapping;
using TrainingService.ViewModels.TrainingProgram;

namespace TrainingService.Features.TrainingProgramFeature.Endpoints;

public static class BrowsePublishedProgramsEndpoint
{
    public static void MapBrowsePublishedProgramsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/programs", async (
            [FromServices] IMediator mediator,
            [AsParameters] BrowsePublishedProgramsQuery request,
            CancellationToken ct) =>
        {
            var result = await mediator.Send(request, ct);

            EndpointResponse<BrowseTrainingProgramsViewModel> response = result.Success
                ? new EndpointResponse<BrowseTrainingProgramsViewModel>(
                    true,                   
                    result.StatusCode,
                    result.Message,
                    result.Data!.ToViewModels())
                : new EndpointResponse<BrowseTrainingProgramsViewModel>(false, result.StatusCode, result.Message);

            return Results.Json(response, statusCode: result.StatusCode);
        })
        .WithName("BrowsePublishedPrograms")
        .WithTags("Programs")
        .AllowAnonymous();
    }
}