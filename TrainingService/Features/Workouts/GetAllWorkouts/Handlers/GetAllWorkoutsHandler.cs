using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetAllWorkouts.Dtos;
using TrainingService.Features.Workouts.GetAllWorkouts.Queries;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Handlers
{
    public class GetAllWorkoutsHandler : IRequestHandler<GetAllWorkoutsQuery, RequestResponse<PagedResult<WorkoutDto>>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;

        public GetAllWorkoutsHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }

        public async Task<RequestResponse<PagedResult<WorkoutDto>>> Handle(GetAllWorkoutsQuery request, CancellationToken cancellationToken)
        {
            var query = _workoutRepo.GetAll()
                         .AsNoTracking()
                         .OrderBy(w => w.Title) 
                         .Select(w => new WorkoutDto(
                             w.Id,
                             w.Title,
                             w.Description,
                             w.Difficulty,
                             w.EstimatedDurationMinutes
                         ));

            var pagedResult = await query.ToPagedResultAsync(request.PageIndex,request.PageSize, cancellationToken);

            return RequestResponse<PagedResult<WorkoutDto>>.Ok(pagedResult, "Workouts retrieved successfully.");
        }
    }
}
