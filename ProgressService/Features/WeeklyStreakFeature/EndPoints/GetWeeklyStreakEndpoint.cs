using MediatR;
using ProgressService.Features.WeeklyStreakFeature.Queries;
using ProgressService.Mapping;
using ProgressService.Shared;
using ProgressService.ViewModels.WeeklyStreak;
using System.Security.Claims;

namespace ProgressService.Features.WeeklyStreakFeature.EndPoints
{
    public static class GetWeeklyStreakEndpoint
    {
        public static void MapGetWeeklyStreak(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/progress/streak", async (ClaimsPrincipal user, IMediator _mediator) =>
            {
                if (!Guid.TryParse(user.FindFirstValue("sub"), out var userId))
                    return Results.Json(EndpointResponse<StreakViewModel>.Fail("Unauthorized", 401), statusCode: 401);

                var result = await _mediator.Send(new GetWeeklyStreakQuery(userId));

                var mappedResult = result.Success && result.Data is not null
                    ? RequestResponse<StreakViewModel>.Ok(result.Data.ToViewModel())
                    : RequestResponse<StreakViewModel>.Fail(result.Message, result.StatusCode, result.Errors);

                return Results.Json(EndpointResponse<StreakViewModel>.FromResult(mappedResult), statusCode: mappedResult.StatusCode);
            })
            .RequireAuthorization();
        } 
    }
}
