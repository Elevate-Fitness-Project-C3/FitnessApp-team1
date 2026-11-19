using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class DeactivateExerciseCommandHandler:IRequestHandler<DeactivateExerciseCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        private readonly IMediator _mediator;
        public DeactivateExerciseCommandHandler(IGenericRepository<Exercise> exerciseRepo, IMediator mediator   )
        {
            _exerciseRepo = exerciseRepo;
            _mediator = mediator;
        }
        public async Task<RequestResponse<Unit>> Handle(DeactivateExerciseCommand request, CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepo.GetByIdAsync(request.ExerciseId);
            if (exercise == null)
                return RequestResponse<Unit>.Fail("Exercise not found or already deactivated", 404);
            
            if (!exercise.IsActive)
                return RequestResponse<Unit>.Ok(Unit.Value);
            
            exercise.IsActive = false;
            _exerciseRepo.SaveInclude(exercise, nameof(Exercise.IsActive));
            await _exerciseRepo.SaveChangesAsync(cancellationToken);
            
            return RequestResponse<Unit>.Ok(Unit.Value);
        }
    }
}
