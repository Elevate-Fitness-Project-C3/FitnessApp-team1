using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.UpdateWorkout.Commands;

namespace TrainingService.Features.Workouts.UpdateWorkout.Handlers
{
    public class UpdateWorkoutHandler : IRequestHandler<UpdateWorkoutCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
        public UpdateWorkoutHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }

        public async Task<RequestResponse<Unit>> Handle(UpdateWorkoutCommand request, CancellationToken cancellationToken)
        {
            var workout = new WorkoutTemplate
            {
                Id = request.WorkoutId,
                Title = request.Title,
                Description = request.Description,
                Difficulty = request.Difficulty,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes
            };

            _workoutRepo.SaveInclude(
                workout,
                nameof(WorkoutTemplate.Title),
                nameof(WorkoutTemplate.Description),
                nameof(WorkoutTemplate.Difficulty),
                nameof(WorkoutTemplate.EstimatedDurationMinutes)
            );

            await _workoutRepo.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Workout updated successfully.");
        }
    }
}
