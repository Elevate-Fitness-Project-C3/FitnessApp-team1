using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class ExerciseExistsQueryHandler:IRequestHandler<ExerciseExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        public ExerciseExistsQueryHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public async Task<RequestResponse<bool>> Handle(ExerciseExistsQuery request, CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepo.GetByIdAsync(request.ExerciseId);
            if (exercise == null)
                return RequestResponse<bool>.Fail("Exercise not found",404);
            
            return RequestResponse<bool>.Ok(true);
        }
    }
}
