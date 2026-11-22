using MediatR;
using ProgressService.Features.WorkoutHistoryFeature.Queries;
using ProgressService.Mapping;
using ProgressService.Shared;
using ProgressService.ViewModels.WorkoutHistory;
using System.Security.Claims;

namespace ProgressService.Features.WorkoutHistoryFeature.EndPoints
{
    public static class GetWorkoutHistoryEndpoint
    {
        public static void MapGetWorkoutHistory(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/progress/history", async (
                ClaimsPrincipal user,
                IMediator mediator,
                int pageIndex = 1,
                int pageSize = 10) =>
            {
                if (!Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
                    return Results.Json(EndpointResponse<PaginatedResult<WorkoutHistoryItemViewModel>>.Fail("Unauthorized", 401),statusCode: 401);

                var result = await mediator.Send(new GetWorkoutHistoryQuery(userId, pageIndex, pageSize));

                var mapped = result.Success && result.Data is not null
                    ? RequestResponse<PaginatedResult<WorkoutHistoryItemViewModel>>.Ok(result.Data.ToViewModel())
                    : RequestResponse<PaginatedResult<WorkoutHistoryItemViewModel>>.Fail(result.Message, result.StatusCode, result.Errors);

                return Results.Json(EndpointResponse<PaginatedResult<WorkoutHistoryItemViewModel>>.FromResult(mapped),statusCode: mapped.StatusCode);
            })
            .RequireAuthorization();
        }
    }
}
