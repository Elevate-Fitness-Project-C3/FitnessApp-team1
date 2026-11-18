using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Dtos;
using TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Queries;

namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Handlers
{
    public class GetWorkoutExercisesHandler : IRequestHandler<GetWorkoutExercisesQuery, RequestResponse<WorkoutWithExercisesDto>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;

        public GetWorkoutExercisesHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }
        public async Task<RequestResponse<WorkoutWithExercisesDto>> Handle(GetWorkoutExercisesQuery request, CancellationToken cancellationToken)
        {
            var workout = await _workoutRepo.GetAll()
             .AsNoTracking()
             .Include(w => w.WorkoutExercises)
                 .ThenInclude(we => we.Exercise)
             .Where(w => w.Id == request.WorkoutId)
             .Select(w => new WorkoutWithExercisesDto(
                 w.Id,
                 w.Title,
                 w.WorkoutExercises
                     .OrderBy(we => we.OrderIndex)
                     .Select(we => new WorkoutExerciseDto(
                         we.ExerciseId,
                         we.Exercise.Name,
                         we.OrderIndex,
                         we.TargetSets,
                         we.TargetReps,
                         we.TargetDurationSeconds
                     )).ToList()
             ))
             .FirstOrDefaultAsync(cancellationToken);

            if (workout == null)
                return RequestResponse<WorkoutWithExercisesDto>.Fail("Workout not found.", 404);
            

            return RequestResponse<WorkoutWithExercisesDto>.Ok(workout, "Workout exercises retrieved successfully.");
        }
    }
}
