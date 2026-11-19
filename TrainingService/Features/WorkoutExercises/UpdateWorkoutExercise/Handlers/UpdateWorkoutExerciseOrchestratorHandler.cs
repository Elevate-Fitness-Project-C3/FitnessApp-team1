using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseExists.Queries;
using TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseInPublishedProgram.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries;
using TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Queries;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Orchestrator;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Handlers
{
    public class UpdateWorkoutExerciseOrchestratorHandler : IRequestHandler<UpdateWorkoutExerciseOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateWorkoutExerciseOrchestratorHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo,IUnitOfWork unitOfWork , IMediator mediator)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(UpdateWorkoutExerciseOrchestrator request, CancellationToken cancellationToken)
        {
            var existsResult = await _mediator.Send(new IsWorkoutExerciseExistsQuery(request.WorkoutExerciseId), cancellationToken);

            if (!existsResult.Success)
                return RequestResponse<Unit>.Fail(existsResult.Message, existsResult.StatusCode);

            var isUsedInPublished = await _mediator.Send(new IsWorkoutExerciseInPublishedProgramQuery(request.WorkoutExerciseId), cancellationToken);

            if (!isUsedInPublished.Success)
                return RequestResponse<Unit>.Fail(isUsedInPublished.Message, isUsedInPublished.StatusCode);

            if (isUsedInPublished.Data)
                return RequestResponse<Unit>.Fail("Cannot update this exercise because its workout is used in a published program.", 400);


            var duplicateOrderResult = await _mediator.Send(new IsWorkoutExerciseOrderDuplicateQuery
                (OrderIndex: request.OrderIndex,WorkoutExerciseId: request.WorkoutExerciseId),cancellationToken);

            if (duplicateOrderResult.Success && duplicateOrderResult.Data)
                return RequestResponse<Unit>.Fail("Duplicate order index for this workout.", 400);

            var workoutExercise = new WorkoutExercise
            {
                Id = request.WorkoutExerciseId,
                OrderIndex = request.OrderIndex,
                TargetSets = request.TargetSets,
                TargetReps = request.TargetReps,
                TargetDurationSeconds = request.TargetDurationSeconds
            };

            _workoutExerciseRepo.SaveInclude(
                workoutExercise,
                nameof(WorkoutExercise.OrderIndex),
                nameof(WorkoutExercise.TargetSets),
                nameof(WorkoutExercise.TargetReps),
                nameof(WorkoutExercise.TargetDurationSeconds)
            );

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Workout exercise updated successfully.");
        }
    }
}
