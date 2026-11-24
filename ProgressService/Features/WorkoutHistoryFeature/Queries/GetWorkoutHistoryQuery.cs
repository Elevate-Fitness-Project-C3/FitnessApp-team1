using MediatR;
using ProgressService.Features.WorkoutHistoryFeature.DTOs;
using ProgressService.Shared;

namespace ProgressService.Features.WorkoutHistoryFeature.Queries
{
    public record GetWorkoutHistoryQuery(Guid UserId,int PageIndex = 1, int PageSize = 10)
        : IRequest<RequestResponse<PaginatedResult<WorkoutHistoryItemDto>>>;
}
