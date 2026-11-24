using MediatR;
using TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Orchestrators;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Endpoints
{
    public static class BrowseTrainingProgramDetailsEndpoint
    {
        public static void MapBrowseTrainingProgramDetailsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/training-programs/{programId:guid}",
    async (
        Guid programId,
        int pageNumber,
        int pageSize,
        IMediator mediator,
        CancellationToken ct) =>
    {
        var result = await mediator.Send(
            new BrowseTrainingProgramDetialsOrchestrator
            (
                programId,
                pageNumber,
                pageSize
            ), ct);

        return Results.Json(result, statusCode: result.StatusCode);
    });
        }
    }
}
