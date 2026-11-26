using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Dtos;
using TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Queries;

namespace TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Controllers
{
    public static class PreviewProgramSelectionEndpoint
    {
        public static void MapPreviewProgramSelectionEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/training-programs/{id:guid}/preview-selection", async (
                Guid id,
                IMediator mediator,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new PreviewProgramSelectionQuery(id), ct);
                var response = EndpointResponse<ProgramSelectionPreviewDto>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("TrainingPrograms")
            .WithName("PreviewProgramSelection")
            .RequireAuthorization();
        }
    }
}
