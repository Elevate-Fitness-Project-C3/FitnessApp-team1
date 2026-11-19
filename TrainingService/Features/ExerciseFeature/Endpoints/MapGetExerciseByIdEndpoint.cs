using MediatR;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;
using TrainingService.Mapping;
using TrainingService.ViewModels.Exercise;

namespace TrainingService.Features.ExerciseFeature.Endpoints
{
    public static class GetExerciseByIdEndpoint
    {
        public static void MapGetExerciseByIdEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/exercises/{id:guid}", async (
                IMediator mediator,
                Guid id,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetExerciseByIdQuery(id), ct);
                var response = result.Success
                    ? EndpointResponse<ExerciseViewModel>.Ok(result.Data!.ToExerciseViewModel())
                    : EndpointResponse<ExerciseViewModel>.FromResult(
                        RequestResponse<ExerciseViewModel>.Fail(
                            result.Message,
                            result.StatusCode));
                return Results.Json(response, statusCode: result.StatusCode);
            })
            .WithName("GetExerciseById")
            .WithTags("Exercises");
        }
    }
}
