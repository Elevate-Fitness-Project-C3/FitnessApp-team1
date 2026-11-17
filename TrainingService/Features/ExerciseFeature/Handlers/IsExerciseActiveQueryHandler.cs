using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;
using TrainingService.Infrastructure.DataAccess;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class IsExerciseActiveQueryHandler:IRequestHandler<IsExerciseActiveQuery,RequestResponse<bool>>
    {
        public readonly IGenericRepository<Exercise> _exerciseRepo;
        public IsExerciseActiveQueryHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public async Task<RequestResponse<bool>> Handle (IsExerciseActiveQuery request, CancellationToken ct)
        {
            var isActive =await _exerciseRepo.GetAll()
                .AnyAsync(e => e.Id == request.ExerciseId,ct);
                
            return RequestResponse<bool>.Fail("dd");
        }
    }
}
