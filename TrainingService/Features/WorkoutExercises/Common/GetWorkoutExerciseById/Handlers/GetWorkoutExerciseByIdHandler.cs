using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries;

namespace TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Handlers
{
    public class GetWorkoutExerciseByIdHandler : IRequestHandler<GetWorkoutExerciseByIdQuery, RequestResponse<WorkoutExercise>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        public GetWorkoutExerciseByIdHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
        }
        public async Task<RequestResponse<WorkoutExercise>> Handle(GetWorkoutExerciseByIdQuery request, CancellationToken cancellationToken)
        {
            var workoutExercise = await _workoutExerciseRepo.GetByIdAsync(request.WorkoutExerciseId);

            if (workoutExercise == null)
                return RequestResponse<WorkoutExercise>.Fail("WorkoutExercise not found.", 404);

            return RequestResponse<WorkoutExercise>.Ok(workoutExercise);
        }
    }
}
