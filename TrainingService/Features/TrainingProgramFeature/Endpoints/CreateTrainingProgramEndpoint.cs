using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Commands;

namespace TrainingService.Features.TrainingProgramFeature.Endpoints
{
    public static class CreateTrainingProgramEndpoint
    {
        public static void MapCreateTrainingProgramEndpoint(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/api/training-programs", async ([AsParameters] CreateTrainingProgramCommand request, IMediator mediator) =>
            {
                var result = await mediator.Send(request);
                var response = EndpointResponse<Guid>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithName("CreateTrainingProgram")
            .WithTags("Training Programs");
            //.RequireAuthorization("Admin"); 
        }
    }
}
