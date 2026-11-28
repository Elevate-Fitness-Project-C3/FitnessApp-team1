using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.UserTrainingPrograms.GetActiveTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries;
using TrainingService.Features.TrainingPrograms.Common.Rules;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Clients;
using TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Commands;
using TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Orchestrator;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Handlers
{
    public class SelectTrainingProgramCommandHandler : IRequestHandler<SelectTrainingProgramOrchestrator, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        private readonly IGenericRepository<UserTrainingProgram> _userProgramRepo;
        private readonly IFitnessProfileClient _profileClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public SelectTrainingProgramCommandHandler(
            IGenericRepository<TrainingProgram> programRepo,
            IGenericRepository<UserTrainingProgram> userProgramRepo,
            IFitnessProfileClient profileClient,
            IUnitOfWork unitOfWork,
            IMediator mediator)
        {
            _programRepo = programRepo;
            _userProgramRepo = userProgramRepo;
            _profileClient = profileClient;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(SelectTrainingProgramOrchestrator request, CancellationToken cancellationToken)
        {

            var scoringResult = await _mediator.Send( new GetProgramScoringDetailsQuery(request.ProgramId), cancellationToken);

            if (!scoringResult.Success)
                return RequestResponse<Unit>.Fail(scoringResult.Message, scoringResult.StatusCode);

            var programDto = scoringResult.Data!;

            if (programDto.Status != ProgramStatus.Published)
                return RequestResponse<Unit>.Fail($"Program is not available (Status: {programDto.Status}).", 409);

            if (programDto.AccessTier == AccessTier.Premium)
                return RequestResponse<Unit>.Fail("Premium requires subscription.", 403);

            var profile = await _profileClient.GetProfileAsync(cancellationToken);

            if (profile is null || !profile.IsOnboardingCompleted)
                return RequestResponse<Unit>.Fail("Please complete your fitness profile first.", 400);

            var activeResult = await _mediator.Send(new GetActiveTrainingProgramQuery(profile.UserId), cancellationToken);

            if (!activeResult.Success)
                return RequestResponse<Unit>.Fail(activeResult.Message, activeResult.StatusCode);

            if (activeResult.Data) 
                return RequestResponse<Unit>.Fail("You have an active training program. Please stop it first.", 409);

            var programEntity = new TrainingProgram
            {
                Id = programDto.ProgramId,
                Title = programDto.Title,
                Goal = programDto.Goal,
                RequiredLevel = programDto.Level,
                DaysPerWeek = programDto.DaysPerWeek,
                Status = programDto.Status,
                AccessTier = programDto.AccessTier
            };

            var (_, warnings) = ProgramRecommendationRules.Calculate(programEntity, profile);

            if (warnings.Count > 0 && !request.AcceptWarnings)
           
                return RequestResponse<Unit>.Fail( "Program has compatibility warnings. Please review and confirm.",400,
                    new Dictionary<string, string[]> { ["Warnings"] = warnings.ToArray() });

            await _unitOfWork.ExecuteAsync(async ct =>
            {

                var stopResult = await _mediator.Send(new StopActiveTrainingProgramCommand(profile.UserId), cancellationToken);

                var newAttempt = new UserTrainingProgram
                {
                    Id = Guid.NewGuid(),
                    UserId = profile.UserId,
                    TrainingProgramId = programDto.ProgramId,
                    Status = AttemptStatus.Active,
                    StartedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                _userProgramRepo.Add(newAttempt);

            }, cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Program selected successfully.");
        }
    }
}

