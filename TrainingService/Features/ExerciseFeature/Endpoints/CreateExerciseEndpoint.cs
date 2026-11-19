using TrainingService.Features.Shared;
using MediatR;
using TrainingService.Features.ExerciseFeature.Commands;

namespace TrainingService.Features.ExerciseFeature.Endpoints
{
    public static class CreateExerciseEndpoint
    {
        public static void MapCreateExerciseEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapPost("/api/exercises", async (IMediator mediator, CreateExerciseCommand request) =>
            {
                var result = await mediator.Send(request);
                var response = EndpointResponse<Guid>.FromResult(result);
                
                return Results.Json(response, statusCode: result.StatusCode);
            })
            .WithName("CreateExercise")
            .WithTags("Exercises")
            .RequireAuthorization("AdminPolicy");
        }
    }
}
