using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseExists.Queries;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseInPublishedProgram.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries;
using TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Orchestrator;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Handlers
{
    public class DeleteWorkoutExerciseOrchestratorHandler : IRequestHandler<DeleteWorkoutExerciseOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteWorkoutExerciseOrchestratorHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutExerciseOrchestrator request, CancellationToken cancellationToken)
        {
            var existsResult = await _mediator.Send(new IsWorkoutExerciseExistsQuery(request.WorkoutExerciseId), cancellationToken);

            if (!existsResult.Success)
                return RequestResponse<Unit>.Fail(existsResult.Message, existsResult.StatusCode);

            var isUsedInPublished = await _mediator.Send(new IsWorkoutExerciseInPublishedProgramQuery(request.WorkoutExerciseId), cancellationToken);

            if (!isUsedInPublished.Success)
                return RequestResponse<Unit>.Fail(isUsedInPublished.Message, isUsedInPublished.StatusCode);

            if (isUsedInPublished.Data)
                return RequestResponse<Unit>.Fail("Cannot delete this exercise because its workout is used in a published program.", 400);


            var workoutExerciseToDelete = new WorkoutExercise
            {
                Id = request.WorkoutExerciseId,
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };

            _workoutExerciseRepo.SaveInclude(
                workoutExerciseToDelete,
                nameof(WorkoutExercise.IsDeleted),
                nameof(WorkoutExercise.DeletedAt)
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Workout exercise deleted successfully.");
        }
    
    }
}
