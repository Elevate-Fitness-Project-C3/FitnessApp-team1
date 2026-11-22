using MediatR;
using ProgressService.Features.WorkoutStatisticsFeature.Queries;
using ProgressService.Mapping;
using ProgressService.Shared;
using ProgressService.ViewModels.WorkoutStatistics;
using System.Security.Claims;

namespace ProgressService.Features.WorkoutStatisticsFeature.EndPoints
{
    public static class GetProgressSummaryEndpoint
    {
        public static void MapGetProgressSummary(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/progress/summary", async (
                ClaimsPrincipal user,
                IMediator mediator) =>
            {
                if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                    return Results.Json(EndpointResponse<ProgressSummaryViewModel>.Fail("Unauthorized", 401),statusCode: 401);

                var result = await mediator.Send(new GetProgressSummaryQuery(userId));

                var mapped = result.Success && result.Data is not null
                    ? RequestResponse<ProgressSummaryViewModel>.Ok(result.Data.ToViewModel())
                    : RequestResponse<ProgressSummaryViewModel>.Fail(result.Message, result.StatusCode, result.Errors);

                return Results.Json(EndpointResponse<ProgressSummaryViewModel>.FromResult(mapped),statusCode: mapped.StatusCode);
            })
            .RequireAuthorization();
        }
    }
}
