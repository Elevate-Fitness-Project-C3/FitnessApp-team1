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
                .FirstOrDefaultAsync(w => w.Id == request.WorkoutId, cancellationToken);

            if (workout == null)
            {
                return RequestResponse<WorkoutByIdDto>.Fail("Workout not found.", 404);
            }

            var dto = new WorkoutByIdDto(
                workout.Id,
                workout.Title,
                workout.Description,
                workout.Difficulty,
                workout.EstimatedDurationMinutes
            );

            return RequestResponse<WorkoutByIdDto>.Ok(dto, "Workout retrieved successfully.");
        }
    }
}
