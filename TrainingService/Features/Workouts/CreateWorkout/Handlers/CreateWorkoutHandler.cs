using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.CreateWorkout.Commands;

namespace TrainingService.Features.Workouts.CreateWorkout.Handlers
{
    public class CreateWorkoutHandler : IRequestHandler<CreateWorkoutCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
        public CreateWorkoutHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
        {
            _workoutRepo = workoutRepo;
        }
        public async Task<RequestResponse<Unit>> Handle(CreateWorkoutCommand command, CancellationToken cancellationToken)
        {
            var workout = new WorkoutTemplate
            {
                Title = command.Title,
                Description = command.Description,
                Difficulty = command.Difficulty,
                EstimatedDurationMinutes = command.EstimatedDurationMinutes
            };

            _workoutRepo.Add(workout);

            await _workoutRepo.SaveChangesAsync(cancellationToken);
            return RequestResponse<Unit>.Created(Unit.Value);
        }
    }
}
