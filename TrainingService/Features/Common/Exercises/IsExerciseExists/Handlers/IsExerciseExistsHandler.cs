using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Exercises.IsExerciseExists.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Exercises.IsExerciseExists.Handlers
{
    public class IsExerciseExistsHandler  : IRequestHandler<IsExerciseExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;

        public IsExerciseExistsHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }

        public async Task<RequestResponse<bool>> Handle(IsExerciseExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _exerciseRepo.ExistsAsync(
                e => e.Id == request.ExerciseId && e.IsActive,
                cancellationToken);

            if (!exists)
                return RequestResponse<bool>.Fail("Exercise not found or inactive.", 404);

            return RequestResponse<bool>.Ok(true, "Exercise exists and is active.");
        }
    }
}
