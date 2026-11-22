using MediatR;
using TrainingService.Domain.Entities;
using Microsoft.EntityFrameworkCore; 
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseInPublishedProgram.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseInPublishedProgram.Handlers
{
    public class IsWorkoutExerciseInPublishedProgramHandler : IRequestHandler<IsWorkoutExerciseInPublishedProgramQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        public IsWorkoutExerciseInPublishedProgramHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsWorkoutExerciseInPublishedProgramQuery request, CancellationToken cancellationToken)
        {
            var isUsed = await _workoutExerciseRepo.GetAll()
                .Where(we => we.Id == request.WorkoutExerciseId)
                .Select(we => (bool?)we.WorkoutTemplate.ProgramWorkoutSlots
                    .Any(slot => slot.TrainingProgram.Status == ProgramStatus.Published))
                .FirstOrDefaultAsync(cancellationToken);

            return isUsed is null
                ? RequestResponse<bool>.Fail("WorkoutExercise not found.", 404)
                : RequestResponse<bool>.Ok(isUsed.Value);
        }
    }

}
