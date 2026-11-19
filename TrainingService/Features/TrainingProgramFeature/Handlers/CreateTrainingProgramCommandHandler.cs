using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Commands;

namespace TrainingService.Features.TrainingProgramFeature.Handlers
{
    public class CreateTrainingProgramCommandHandler : IRequestHandler<CreateTrainingProgramCommand, RequestResponse<Guid>>
    {
        private readonly IGenericRepository<TrainingProgram> _trainingProgramRepo;
        public CreateTrainingProgramCommandHandler(IGenericRepository<TrainingProgram> trainingProgramRepo)
        {
            _trainingProgramRepo = trainingProgramRepo;
        }
        public async Task<RequestResponse<Guid>> Handle(CreateTrainingProgramCommand request, CancellationToken ct)
        {
            var isTitleTaken = await _trainingProgramRepo.ExistsAsync(tp => tp.Title == request.Title, ct);
            if (isTitleTaken)
                return RequestResponse<Guid>.Fail("Training program title is already taken.", 409);
            

            if (!Enum.IsDefined(request.FitnessGoal))
                return RequestResponse<Guid>.Fail("Invalid fitness goal.", 400);

            if (!Enum.IsDefined(request.ExperienceLevel))
                return RequestResponse<Guid>.Fail("Invalid experience level.", 400);

            if (!Enum.IsDefined(request.AccessTier))
                return RequestResponse<Guid>.Fail("Invalid access tier.", 400);

            if (request.DaysPerWeek < 1 || request.DaysPerWeek > 7)
                return RequestResponse<Guid>.Fail("Days per week must be between 1 and 7.", 400);
            if (request.DurationWeeks < 1)
                return RequestResponse<Guid>.Fail("Duration in weeks must be at least 1.", 400);

            var trainingProgram = new TrainingProgram
            {
                Title = request.Title,
                Description = request.Description,
                Goal = request.FitnessGoal,
                RequiredLevel = request.ExperienceLevel,
                DurationWeeks = request.DurationWeeks,
                DaysPerWeek = request.DaysPerWeek,
                AccessTier = request.AccessTier,
                Status = ProgramStatus.Draft //published by default, which is better when creating a new program
            };
            _trainingProgramRepo.Add(trainingProgram);
            await _trainingProgramRepo.SaveChangesAsync(ct);

            return RequestResponse<Guid>.Created(trainingProgram.Id, "Training program created successfully.");
        }
    }
}
