using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Exercises.IsExerciseExists.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Queries;
using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Orchestrators;

namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Handlers
{
    public class CreateExerciseToWorkoutOrchestratorHandler : IRequestHandler<CreateExerciseToWorkoutOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;


        public CreateExerciseToWorkoutOrchestratorHandler(
        IGenericRepository<WorkoutExercise> workoutExerciseRepo,
        IMediator mediator,
        IUnitOfWork unitOfWork)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<Unit>> Handle(CreateExerciseToWorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var workoutExists = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);
            if (!workoutExists.Success)
                return RequestResponse<Unit>.Fail(workoutExists.Message, workoutExists.StatusCode);

            var exerciseExists = await _mediator.Send(new IsExerciseExistsQuery(request.ExerciseId),cancellationToken);

            if (!exerciseExists.Success)
                return RequestResponse<Unit>.Fail(exerciseExists.Message, exerciseExists.StatusCode);

            var isUsedInPublished = await _mediator.Send( new IsWorkoutUsedInPublishedProgramQuery(request.WorkoutId),cancellationToken);

            if (!isUsedInPublished.Success)
                return RequestResponse<Unit>.Fail(isUsedInPublished.Message, isUsedInPublished.StatusCode);

            if (isUsedInPublished.Data)
                return RequestResponse<Unit>.Fail("Cannot add exercises to a workout used in a published program.", 400);


            var duplicateOrderResult = await _mediator.Send(new IsWorkoutExerciseOrderDuplicateQuery(
                    request.OrderIndex,
                    request.WorkoutId,
                    null               
                ), cancellationToken);

            if (!duplicateOrderResult.Success)
                return RequestResponse<Unit>.Fail(duplicateOrderResult.Message, duplicateOrderResult.StatusCode);

            if (duplicateOrderResult.Data)
                return RequestResponse<Unit>.Fail("Duplicate order index for this workout.", 400);

            var workoutExercise = new WorkoutExercise
            {
                WorkoutTemplateId = request.WorkoutId,
                ExerciseId = request.ExerciseId,
                OrderIndex = request.OrderIndex,
                TargetSets = request.TargetSets,
                TargetReps = request.TargetReps,
                TargetDurationSeconds = request.TargetDurationSeconds,
                CreatedAt = DateTime.UtcNow,

            };

            _workoutExerciseRepo.Add(workoutExercise);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Created(Unit.Value, "Exercise added to workout successfully.");
        }
    

    }
}
