using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.Entities;
using ProgressService.Domain.IRepositories;
using ProgressService.Features.WorkoutStatisticsFeature.DTOs;
using ProgressService.Features.WorkoutStatisticsFeature.Queries;
using ProgressService.Shared;

namespace ProgressService.Features.WorkoutStatisticsFeature.Handlers
{
    public class GetProgressSummaryQueryHandler : IRequestHandler<GetProgressSummaryQuery, RequestResponse<ProgressSummaryDto>>
    {
        private readonly IGenericRepository<CompletedWorkoutRecord> _repository;

        public GetProgressSummaryQueryHandler(IGenericRepository<CompletedWorkoutRecord> repository)
            => _repository = repository;
        public async Task<RequestResponse<ProgressSummaryDto>> Handle(GetProgressSummaryQuery request, CancellationToken cancellationToken)
        {
            //1. Get all completed workout records for the user :
            var UserRecords = _repository.GetAll()
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            //2. Calculate the total number of completed workouts :
            var totalCompletedWorkouts = await UserRecords.CountAsync(cancellationToken);

            //3.Calculate the date of the last completed workout :
            var lastWorkoutAt = await UserRecords.MaxAsync(x => (DateTime?)x.CompletedAtUtc, cancellationToken);

            //4. Create a Summary DTO with the calculated values :
            var summaryDto = new ProgressSummaryDto
            {
                TotalCompletedWorkouts = totalCompletedWorkouts,
                LastWorkoutAtUtc = lastWorkoutAt
            };

            return RequestResponse<ProgressSummaryDto>.Ok(summaryDto);
        }
    }
}
