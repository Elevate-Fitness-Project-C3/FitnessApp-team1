using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Queries;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Endpoints
{
    public static class GetProgramRecommendationsEndpoint
    {
        public static void MapGetProgramRecommendationsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/training-programs/recommendations", async (IMediator mediator,CancellationToken ct) =>
            {
                var result = await mediator.Send(new GetProgramRecommendationsQuery(), ct);

                var response = EndpointResponse<List<ProgramRecommendationDto>>.FromResult(result);
                return Results.Json(response, statusCode: response.StatusCode);
            })
                .WithTags("TrainingPrograms")
                .WithName("ProgramRecommendations")
                .RequireAuthorization();
        }
    }
}
