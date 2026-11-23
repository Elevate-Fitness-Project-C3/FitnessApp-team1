
using MediatR;
using TrainingService.Features.SetTrainingRefrence.Command;
using TrainingService.Features.SetTrainingRefrence.DTO;

namespace TrainingService.Features.SetTrainingRefrence.EndPoint;

public static class TrainingPreferencesEndpoint
{
    public static IEndpointRouteBuilder MapTrainingPreferencesEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPut(
            "/api/fitnessprofile/trainingpreferences",
            async (
                TrainingPreferencesDto trainingPreferencesDto,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            {
                var command = new UpdateTrainingPreferencesCommand(
                    trainingPreferencesDto);

                var result = await mediator.Send(
                    command,
                    cancellationToken);

                return Results.Ok(
                    new
                    {
                        success = result.Succeeded,
                        message = result.Message
                    }
                    );
            })
            .RequireAuthorization()
            .WithTags("Fitness Profile");

        return app;
    }
}
