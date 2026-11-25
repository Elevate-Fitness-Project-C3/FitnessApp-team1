using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.Entities;
using ProgressService.Domain.IRepositories;
using ProgressService.Features.WeeklyStreakFeature.Calculators;
using ProgressService.Features.WeeklyStreakFeature.DTOs;
using ProgressService.Features.WeeklyStreakFeature.Queries;
using ProgressService.Shared;

namespace ProgressService.Features.WeeklyStreakFeature.Handlers
{
    public class GetWeeklyStreakQueryHandler : IRequestHandler<GetWeeklyStreakQuery, RequestResponse<StreakDto>>
    {
        private readonly IGenericRepository<CompletedWorkoutRecord> _repository;
        private readonly TimeProvider _time;

        public GetWeeklyStreakQueryHandler(IGenericRepository<CompletedWorkoutRecord> repository, TimeProvider time)
        {
            _repository = repository;
            _time = time;
        }
        public async Task<RequestResponse<StreakDto>> Handle(GetWeeklyStreakQuery request, CancellationToken cancellationToken)
        {
            //1. Get all completed workout records for the user
            var timestamps = await _repository.GetAll()
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId)
                .Select(x => x.CompletedAtUtc)
                .ToListAsync(cancellationToken);

            //2. 
            var result = WeeklyStreakCalculator.Calculate(
                timestamps,
                _time.GetUtcNow().UtcDateTime
                , GetCairo());

            //3.
            return RequestResponse<StreakDto>.Ok(new StreakDto
            {
                CurrentStreak = result.CurrentStreak,
                LongestStreak = result.LongestStreak
            });
        }

        private static TimeZoneInfo GetCairo()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo"); }
            catch (TimeZoneNotFoundException) { return TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time"); }
        }
    }
}
