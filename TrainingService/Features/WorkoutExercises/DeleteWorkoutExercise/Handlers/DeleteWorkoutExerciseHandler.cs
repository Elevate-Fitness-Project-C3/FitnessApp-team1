using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries;
using TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Commands;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Handlers
{
    public class DeleteWorkoutExerciseHandler : IRequestHandler<DeleteWorkoutExerciseCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteWorkoutExerciseHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo, IMediator mediator, IUnitOfWork unitOfWork)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutExerciseCommand request, CancellationToken cancellationToken)
        {
            var workoutExerciseResult = await _mediator.Send(
                      new GetWorkoutExerciseByIdQuery(request.WorkoutExerciseId),cancellationToken);

            if (workoutExerciseResult.Data == null)
                return RequestResponse<Unit>.Fail(workoutExerciseResult.Message, workoutExerciseResult.StatusCode);

            var workoutExercise = workoutExerciseResult.Data;

            var isUsedInPublished = await _mediator.Send(
                new IsWorkoutUsedInPublishedProgramQuery(workoutExercise.WorkoutTemplateId),
                cancellationToken);

            if (isUsedInPublished.Data)
                return RequestResponse<Unit>.Fail("Cannot delete this exercise because its workout is used in a published program.",400);


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
