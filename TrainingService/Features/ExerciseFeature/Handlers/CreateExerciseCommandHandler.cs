using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ExerciseFeature.Commands;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class CreateExerciseCommandHandler : IRequestHandler<CreateExerciseCommand, RequestResponse<Guid>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepo;
        private readonly IMediator _mediator;

        public CreateExerciseCommandHandler(IGenericRepository<Exercise> exerciseRepo, IMediator mediator   )
        {
            _exerciseRepo = exerciseRepo;
            _mediator = mediator;
        }
        public async Task<RequestResponse<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken)
        {
            var isNameTakenResponse = await _mediator.Send(new IsExerciseNameTakenQuery(request.Name), cancellationToken);
            if(isNameTakenResponse.Data)
                return RequestResponse<Guid>.Fail("Exercise name is already taken.", 409);
            
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