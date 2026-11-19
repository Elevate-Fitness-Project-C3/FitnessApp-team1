using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.IsWorkoutExerciseOrderDuplicate.Queries;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Commands;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Handlers
{
    public class UpdateWorkoutExerciseHandler : IRequestHandler<UpdateWorkoutExerciseCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;

        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateWorkoutExerciseHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo,IUnitOfWork unitOfWork , IMediator mediator)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(UpdateWorkoutExerciseCommand request, CancellationToken cancellationToken)
        {

            var exists = await _workoutExerciseRepo.ExistsAsync(
             we => we.Id == request.WorkoutExerciseId,
             cancellationToken);

            if (!exists)
                return RequestResponse<Unit>.Fail("WorkoutExercise not found.", 404);

            var duplicateOrderResult = await _mediator.Send(
               new IsWorkoutExerciseOrderDuplicateQuery(
               OrderIndex: request.OrderIndex,
               WorkoutExerciseId: request.WorkoutExerciseId),cancellationToken);

            if (duplicateOrderResult.Data)
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
