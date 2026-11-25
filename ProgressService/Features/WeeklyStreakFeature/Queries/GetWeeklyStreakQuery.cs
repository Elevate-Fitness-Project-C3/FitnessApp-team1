using MediatR;
using ProgressService.Features.WeeklyStreakFeature.DTOs;
using ProgressService.Shared;

namespace ProgressService.Features.WeeklyStreakFeature.Queries
{
    public record GetWeeklyStreakQuery(Guid UserId) : IRequest<RequestResponse<StreakDto>>;
}
