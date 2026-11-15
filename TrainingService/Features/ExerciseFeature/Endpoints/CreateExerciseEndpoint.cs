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
                var response = await mediator.Send(request);
                if (!response.Success)
                    return Results.BadRequest(EndpointResponse<Guid>.Fail(response.Message));

                return Results.Created($"api/exercises/{response.Data}", EndpointResponse<Guid>.Created(response.Data));
            })
            .WithName("CreateExercise")
            .WithTags("Exercises");
        }
    }
}
