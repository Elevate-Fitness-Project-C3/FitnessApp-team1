using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseExists.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseExists.Handlers
{
    public class IsWorkoutExerciseExistsHandler : IRequestHandler<IsWorkoutExerciseExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        public IsWorkoutExerciseExistsHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsWorkoutExerciseExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _workoutExerciseRepo.ExistsAsync(
                we => we.Id == request.WorkoutExerciseId,
                cancellationToken);

            if (!exists)
                return RequestResponse<bool>.Fail("WorkoutExercise not found.", 404);

            return RequestResponse<bool>.Ok(true, "WorkoutExercise exists.");
        }
    }
}
