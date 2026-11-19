using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.UpdateWorkout.Orchestrator;

namespace TrainingService.Features.Workouts.UpdateWorkout.Handlers
{
    public class UpdateWorkoutOrchestratorHandler : IRequestHandler<UpdateWorkoutOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
        private IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public UpdateWorkoutOrchestratorHandler(IGenericRepository<WorkoutTemplate> workoutRepo, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _workoutRepo = workoutRepo;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(UpdateWorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var existsResult = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);

            if (!existsResult.Success)
                return RequestResponse<Unit>.Fail(existsResult.Message, existsResult.StatusCode);

            var isUsedInPublished = await _mediator.Send(new IsWorkoutUsedInPublishedProgramQuery(request.WorkoutId), cancellationToken);


            if (!isUsedInPublished.Success)
                return RequestResponse<Unit>.Fail(isUsedInPublished.Message, isUsedInPublished.StatusCode);

            if (isUsedInPublished.Data)
                return RequestResponse<Unit>.Fail("Cannot update this workout because it is used in a published program.", 400);


            var workout = new WorkoutTemplate
            {
                Id = request.WorkoutId,
                Title = request.Title,
                Description = request.Description,
                Difficulty = request.Difficulty,
                EstimatedDurationMinutes = request.EstimatedDurationMinutes,
                UpdatedAt = DateTime.UtcNow
            };

            _workoutRepo.SaveInclude(
                workout,
                nameof(WorkoutTemplate.Title),
                nameof(WorkoutTemplate.Description),
                nameof(WorkoutTemplate.Difficulty),
                nameof(WorkoutTemplate.EstimatedDurationMinutes),
                nameof(WorkoutTemplate.UpdatedAt)
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Workout updated successfully.");
        }
    }
}