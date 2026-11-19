using MediatR;
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
                IMediator mediator,
                [AsParameters] BrowseExercisesQuery request,
                CancellationToken ct = default) =>
            {
                var result = await mediator.Send(request,ct);

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
