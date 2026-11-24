using MediatR;
using ProgressService.Features.WorkoutStatisticsFeature.DTOs;
using ProgressService.Shared;

namespace ProgressService.Features.WorkoutStatisticsFeature.Queries
{
    public record GetProgressSummaryQuery(Guid UserId) : IRequest<RequestResponse<ProgressSummaryDto>>;
}
