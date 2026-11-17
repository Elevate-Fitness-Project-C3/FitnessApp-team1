using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class IsExerciseNameTakenQueryHandler:IRequestHandler<IsExerciseNameTakenQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        public IsExerciseNameTakenQueryHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public async Task<RequestResponse<bool>> Handle(IsExerciseNameTakenQuery request, CancellationToken cancellationToken)
        {
            var isNameTaken = await _exerciseRepo.ExistsAsync(e => e.Name == request.Name, cancellationToken);
            return RequestResponse<bool>.Ok(isNameTaken);
        }
    }
}
