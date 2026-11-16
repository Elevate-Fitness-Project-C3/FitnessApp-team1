using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Queries;
using TrainingService.Features.Workouts.UpdateWorkout.Commands;

namespace TrainingService.Features.Workouts.UpdateWorkout.Handlers
{
    public class UpdateWorkoutHandler : IRequestHandler<UpdateWorkoutCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
        private readonly IMediator _mediator;

        public UpdateWorkoutHandler(IGenericRepository<WorkoutTemplate> workoutRepo, IMediator mediator)
        {
            _workoutRepo = workoutRepo;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(UpdateWorkoutCommand request, CancellationToken cancellationToken)
        {
            var existsResult = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);

            if (!existsResult.Success)
                return RequestResponse<Unit>.Fail(existsResult.Message, existsResult.StatusCode);

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