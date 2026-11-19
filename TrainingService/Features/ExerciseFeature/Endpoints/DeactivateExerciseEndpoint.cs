using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Endpoints
{
    public static class DeactivateExerciseEndpoint
    {
        public static void MapDeactivateExerciseEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPatch("/api/exercises/{exerciseId:guid}/deactivate", async (
                [FromServices] IMediator mediator,
                [FromRoute] Guid exerciseId,
                CancellationToken ct) =>
            {
                var result = await mediator.Send(new DeactivateExerciseCommand(exerciseId), ct);
                var response = EndpointResponse<Unit>.FromResult(result);

                return Results.Json(response, statusCode: result.StatusCode);
            })
            .WithName("DeactivateExercise")
            .WithTags("Exercises")
            .RequireAuthorization("AdminPolicy");

        }
    }
}