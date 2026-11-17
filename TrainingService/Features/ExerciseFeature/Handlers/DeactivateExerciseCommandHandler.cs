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
            var isExerciseActiveResponse = await _mediator.Send(new ExerciseExistsQuery(request.ExerciseId), cancellationToken);
            if(!isExerciseActiveResponse.Data)
                return RequestResponse<Unit>.Fail("Exercise not found", 404);
            var isExerciseActive = await _mediator.Send(new IsExerciseActiveQuery(request.ExerciseId), cancellationToken);
            if(!isExerciseActive.Data)
                return RequestResponse<Unit>.Fail("Exercise is already deactivated", 400);
            await _exerciseRepo.DeleteAsync(request.ExerciseId,cancellationToken);
            return RequestResponse<Unit>.Ok(Unit.Value);
        }
    }
}
