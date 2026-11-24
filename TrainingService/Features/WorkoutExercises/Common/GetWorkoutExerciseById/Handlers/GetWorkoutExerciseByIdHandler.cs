using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Dtos;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries;

namespace TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Handlers
{
    public class GetWorkoutExerciseByIdHandler : IRequestHandler<GetWorkoutExerciseByIdQuery, RequestResponse<WorkoutExerciseDto>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        public GetWorkoutExerciseByIdHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
        }
        public async Task<RequestResponse<WorkoutExerciseDto>> Handle(GetWorkoutExerciseByIdQuery request, CancellationToken cancellationToken)
        {
            var query = await _workoutExerciseRepo.GetAll()
                .Where(we => we.Id == request.WorkoutExerciseId)
                .Select(we => new WorkoutExerciseDto(
                    we.Id,
                    we.ExerciseId,
                    we.Exercise.Name,
                    we.OrderIndex,
                    we.TargetSets,
                    we.TargetReps,
                    we.TargetDurationSeconds
                ))
                .FirstOrDefaultAsync(cancellationToken);

            if (query == null)
                return RequestResponse<WorkoutExerciseDto>.Fail("WorkoutExercise not found.", 404);

            return RequestResponse<WorkoutExerciseDto>.Ok(query);
        }
    }
}
