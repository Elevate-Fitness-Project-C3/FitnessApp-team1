using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, RequestResponse<Guid>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        
        public CreateExerciseCommandHandler(IGenericRepository<Exercise> exerciseRepo)
        {
            _exerciseRepo = exerciseRepo;
        }
        public async Task<RequestResponse<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            var exercise = new Exercise
            {
                Name = request.Name,
                Description = request.Description ?? string.Empty,
                TargetMuscle = Enum.Parse<MuscleGroup>(request.TargetMuscle),
                TrackingType = Enum.Parse<TrackingType>(request.TrackingType),
                IsActive = request.IsActive
            };
            _exerciseRepo.Add(exercise);
            await _exerciseRepo.SaveChangesAsync(cancellationToken);
            return RequestResponse<Guid>.Created(exercise.Id);
        }
    }
}