using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetWorkoutById.Dtos;
using TrainingService.Features.Workouts.GetWorkoutById.Queries;

namespace TrainingService.Features.Workouts.GetWorkoutById.Handlers
{
    public class GetWorkoutByIdHandler : IRequestHandler<GetWorkoutByIdQuery, RequestResponse<WorkoutByIdDto>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;

        public GetWorkoutByIdHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }

        public async Task<RequestResponse<WorkoutByIdDto>> Handle(GetWorkoutByIdQuery request, CancellationToken cancellationToken)
        {
            var workout = await _workoutRepo.GetAll()
                .AsNoTracking()
                .Where(w => w.Id == request.WorkoutId)
                .Select(w => new WorkoutByIdDto(
                    w.Id,
                    w.Title,
                    w.Description,
                    w.Difficulty,
                    w.EstimatedDurationMinutes
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (workout == null)
                return RequestResponse<WorkoutByIdDto>.Fail("Workout not found.", 404);

            return RequestResponse<WorkoutByIdDto>.Ok(workout, "Workout retrieved successfully.");
        }
    }
}
