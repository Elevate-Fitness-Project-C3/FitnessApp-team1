using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class UpdateExerciseDescriptionCommandHandler : IRequestHandler<UpdateExerciseDescriptionCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepository;
        public UpdateExerciseDescriptionCommandHandler(IGenericRepository<Exercise> exerciseRepository)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResponse<Unit>> Handle(UpdateExerciseDescriptionCommand request, CancellationToken ct)
        {
            var exercise = await _exerciseRepository.GetByIdAsync(request.ExerciseId);
            if (exercise == null)
                return RequestResponse<Unit>.Fail("Exercise not found.", 404);
            
            exercise.Description = request.Description;
            _exerciseRepository.SaveInclude(exercise, nameof(Exercise.Description));
            await _exerciseRepository.SaveChangesAsync(ct);

            return RequestResponse<Unit>.Ok(Unit.Value, "Exercise description updated successfully.");
        }
    }
}
