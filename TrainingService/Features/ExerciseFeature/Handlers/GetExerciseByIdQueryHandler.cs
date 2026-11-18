using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class GetExerciseByIdQueryHandler:IRequestHandler<GetExerciseByIdQuery, RequestResponse<ExerciseDto>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        public GetExerciseByIdQueryHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public async Task<RequestResponse<ExerciseDto>> Handle(GetExerciseByIdQuery request, CancellationToken cancellationToken)
        {
            var exercise = await _exerciseRepo.GetByIdAsync(request.Id);
            if (exercise == null)
                return RequestResponse<ExerciseDto>.Fail("Exercise not found", 404);
            
            var exerciseDto = new ExerciseDto
            {
                Id = exercise.Id,
                Name = exercise.Name,
                Description = exercise.Description,
                TargetMuscle = exercise.TargetMuscle,
                TrackingType = exercise.TrackingType,
            };
            return RequestResponse<ExerciseDto>.Ok(exerciseDto);
        }
    }
}
