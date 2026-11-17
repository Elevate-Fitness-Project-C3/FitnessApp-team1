using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;
using TrainingService.Mapping;
using TrainingService.ViewModels.Exercise;

namespace TrainingService.Features.ExerciseFeature.Endpoints
{
    public static class BrowseExercisesEndpoint
    {
        public static void MapBrowseExercisesEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/exercises", async (
                [FromServices] IMediator mediator,
                [FromQuery] string? name,
                [FromQuery] string? targetMuscle,
                [FromQuery] string? trackingType,
                [FromQuery] int pageIndex = 1,
                [FromQuery] int pageSize = 10,
                CancellationToken ct = default) =>
            {
                var result = await mediator.Send(
                    new BrowseExercisesQuery(
                        name, targetMuscle, trackingType, pageIndex, pageSize),
                    ct);

                var response = result.Success
                    ? EndpointResponse<BrowseExercisesViewModel>.Ok(
                        result.Data!.ToBrowseExercisesViewModel())
                    : EndpointResponse<BrowseExercisesViewModel>.FromResult(
                        RequestResponse<BrowseExercisesViewModel>.Fail(
                            result.Message,
                            result.StatusCode));

                return Results.Json(response, statusCode: result.StatusCode);
            })
            .WithName("BrowseExercises")
            .WithTags("Exercises");
        }
    }
}
