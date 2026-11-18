using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Commands;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Handlers
{
    public class DeleteWorkoutExerciseHandler : IRequestHandler<DeleteWorkoutExerciseCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<WorkoutExercise> _workoutExerciseRepo;
        private readonly IUnitOfWork _unitOfWork;
        public DeleteWorkoutExerciseHandler(IGenericRepository<WorkoutExercise> workoutExerciseRepo, IUnitOfWork unitOfWork)
        {
            _workoutExerciseRepo = workoutExerciseRepo;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutExerciseCommand request, CancellationToken cancellationToken)
        {
            var exists = await _workoutExerciseRepo.ExistsAsync(we => we.Id == request.WorkoutExerciseId,cancellationToken);

            if (!exists)
                return RequestResponse<Unit>.Fail("WorkoutExercise not found.", 404);

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
