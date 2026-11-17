using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Endpoints
{
    public static class UpdateExerciseDescriptionEndpoint
    {
        public static void MapUpdateExerciseDescriptionEndpoint(this IEndpointRouteBuilder app)
        {
            //exerciseId is passed in the URL
            //the request body contains the new description in the form of UpdateExerciseDescriptionCommand
            app.MapPut("/api/exercises/{exerciseId:guid}/description", async (IMediator mediator,[FromRoute] Guid exerciseId,[FromBody] string description) =>
            {
                var result = await mediator.Send(new UpdateExerciseDescriptionCommand(exerciseId, description));

                var response = EndpointResponse<Unit>.FromResult(result);
                return Results.Json(response, statusCode: result.StatusCode);
            })
            .WithName("UpdateExerciseDescription")
            .WithTags("Exercises")
            .RequireAuthorization("AdminPolicy");
      
        }
    }
}
