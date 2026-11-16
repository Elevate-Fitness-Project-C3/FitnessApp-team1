using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Queries;

namespace TrainingService.Features.Workouts.DeleteWorkout.Handlers
{
    public class IsWorkoutExistsHandler : IRequestHandler<IsWorkoutExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;

        public IsWorkoutExistsHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsWorkoutExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _workoutRepo.ExistsAsync(w => w.Id == request.WorkoutId && !w.IsDeleted, cancellationToken);

           return exists
           ? RequestResponse<bool>.Ok(true, "Workout exists.")
           : RequestResponse<bool>.Fail("Workout not found.", 404);
        }
    }
}
