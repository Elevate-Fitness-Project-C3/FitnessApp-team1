using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.Entities;
using ProgressService.Domain.IRepositories;
using ProgressService.Features.WorkoutHistoryFeature.DTOs;
using ProgressService.Features.WorkoutHistoryFeature.Queries;
using ProgressService.Mapping;
using ProgressService.Shared;

namespace ProgressService.Features.WorkoutHistoryFeature.Handlers
{
    public class GetWorkoutHistoryQueryHandler : IRequestHandler<GetWorkoutHistoryQuery, RequestResponse<PaginatedResult<WorkoutHistoryItemDto>>>
    {
        private readonly IGenericRepository<CompletedWorkoutRecord> _repository;

        public GetWorkoutHistoryQueryHandler(IGenericRepository<CompletedWorkoutRecord> repository)
            => _repository = repository;
        public async Task<RequestResponse<PaginatedResult<WorkoutHistoryItemDto>>> Handle(GetWorkoutHistoryQuery request, CancellationToken cancellationToken)
        {
            //1.Paginate the completed workout records for the user :
            var pageIndex = Math.Max(request.PageIndex, 1);
            var pageSize = Math.Clamp(request.PageSize, 1, 50); // Assuming a maximum page size of 50

            //2.Get the completed workout records :
            var userWorkoutRecords = _repository.GetAll()
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            //3.Get the total count of completed workout records for the user :
            var totalCount = await userWorkoutRecords.CountAsync(cancellationToken);

            //4.Paginate the records & Map the records to DTOs :
            var items = await userWorkoutRecords
               .OrderByDescending(x => x.CompletedAtUtc)
               .ThenByDescending(x => x.Id)
               .Skip((pageIndex - 1) * pageSize)
               .Take(pageSize)
               .Select(x => new WorkoutHistoryItemDto
               {
                   Id = x.Id,
                   WorkoutSessionId = x.WorkoutSessionId,
                   TrainingProgramId = x.TrainingProgramId,
                   ProgramWorkoutSlotId = x.ProgramWorkoutSlotId,
                   CompletedAtUtc = x.CompletedAtUtc
               })
               .ToListAsync(cancellationToken);

            //5.Return the paginated result :
            var page = PaginatedResult<WorkoutHistoryItemDto>.Create(items, totalCount, pageIndex, pageSize);
            return RequestResponse<PaginatedResult<WorkoutHistoryItemDto>>.Ok(page);
        }
    }
}
