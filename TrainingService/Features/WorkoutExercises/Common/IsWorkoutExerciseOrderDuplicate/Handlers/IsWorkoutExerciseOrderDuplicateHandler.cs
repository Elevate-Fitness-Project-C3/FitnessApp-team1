using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Queries;

namespace TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Handlers
{
    public class IsWorkoutExerciseOrderDuplicateHandler: IRequestHandler<IsWorkoutExerciseOrderDuplicateQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        public IsWorkoutExerciseOrderDuplicateHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
        }

        public async Task<RequestResponse<bool>> Handle(
            IsWorkoutExerciseOrderDuplicateQuery request, CancellationToken cancellationToken)
        {
            var workoutId = request.WorkoutId;

            if (workoutId is null)
            {
                if (request.WorkoutExerciseId is null)
                    return RequestResponse<bool>.Fail("WorkoutId or WorkoutExerciseId is required.", 400);

                workoutId = await _workoutExerciseRepo.GetAll()
                    .Where(we => we.Id == request.WorkoutExerciseId)
                    .Select(we => (Guid?)we.WorkoutTemplateId)
                    .FirstOrDefaultAsync(cancellationToken);

                if (workoutId is null)
                    return RequestResponse<bool>.Fail("WorkoutExercise not found.", 404);
            }
            var workoutid = workoutId.Value;
            var excludeWorkoutExerciseId = request.WorkoutExerciseId;

            var isTaken = await _workoutExerciseRepo.ExistsAsync(
                we => we.WorkoutTemplateId == workoutid
                      && we.OrderIndex == request.OrderIndex
                      && (excludeWorkoutExerciseId == null || we.Id != excludeWorkoutExerciseId),
                cancellationToken);

            return RequestResponse<bool>.Ok(isTaken);
        }
    }
}