using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Orchestrators;

namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Handlers
{
    public class CreateExerciseToWorkoutOrchestratorHandler : IRequestHandler<CreateExerciseToWorkoutOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;


        public CreateExerciseToWorkoutOrchestratorHandler(
        IGenericRepository<WorkoutExercise> workoutExerciseRepo,
        IGenericRepository<Exercise> exerciseRepo,
        IMediator mediator,
        IUnitOfWork unitOfWork)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _exerciseRepo = exerciseRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<Unit>> Handle(CreateExerciseToWorkoutOrchestrator request, CancellationToken cancellationToken)
        {
            var workoutExists = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);
            if (!workoutExists.Success)
                return RequestResponse<Unit>.Fail(workoutExists.Message, workoutExists.StatusCode);

            var exerciseExists = await _exerciseRepo.ExistsAsync(
                e => e.Id == request.ExerciseId && e.IsActive,
                cancellationToken);

            if (!exerciseExists)
                return RequestResponse<Unit>.Fail("Exercise not found or inactive.", 404);

            var orderIndexExists = await _workoutExerciseRepo.ExistsAsync(
                we => we.WorkoutTemplateId == request.WorkoutId && we.OrderIndex == request.OrderIndex,
                cancellationToken);

            if (orderIndexExists)
                return RequestResponse<Unit>.Fail("Duplicate order index for this workout.", 400);

            var workoutExercise = new WorkoutExercise
            {
                WorkoutTemplateId = request.WorkoutId,
                ExerciseId = request.ExerciseId,
                OrderIndex = request.OrderIndex,
                TargetSets = request.TargetSets,
                TargetReps = request.TargetReps,
                TargetDurationSeconds = request.TargetDurationSeconds
            };

            _workoutExerciseRepo.Add(workoutExercise);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Created(Unit.Value, "Exercise added to workout successfully.");
        }
    

    }
}
