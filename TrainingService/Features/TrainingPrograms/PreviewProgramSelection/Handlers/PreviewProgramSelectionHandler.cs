using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries;
using TrainingService.Features.TrainingPrograms.Common.Rules;
using TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Dtos;
using TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Queries;
using TrainingService.Infrastructure.Clients;

namespace TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Handlers
{
    public class PreviewProgramSelectionHandler : IRequestHandler<PreviewProgramSelectionQuery, RequestResponse<ProgramSelectionPreviewDto>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        private readonly IFitnessProfileClient _profileClient;
        private readonly IMediator _mediator;

        public PreviewProgramSelectionHandler(
           IGenericRepository<TrainingProgram> programRepo,
           IFitnessProfileClient profileClient,
           IMediator mediator)
        {
            _programRepo = programRepo;
            _profileClient = profileClient;
            _mediator = mediator;
        }
        public async Task<RequestResponse<ProgramSelectionPreviewDto>> Handle(PreviewProgramSelectionQuery request, CancellationToken cancellationToken)
        {
            var scoringResult = await _mediator.Send(new GetProgramScoringDetailsQuery(request.ProgramId), cancellationToken);

            if (!scoringResult.Success)
                return RequestResponse<ProgramSelectionPreviewDto>.Fail(scoringResult.Message, scoringResult.StatusCode);

            var program = scoringResult.Data!;

            var programEntity = new TrainingProgram
            {
                Id = program.ProgramId,
                Title = program.Title,
                Goal = program.Goal,
                RequiredLevel = program.Level,
                DaysPerWeek = program.DaysPerWeek,
                Status = program.Status,
                AccessTier = program.AccessTier
            };

            var profile = await _profileClient.GetProfileAsync(cancellationToken);
            if (profile is null || !profile.IsOnboardingCompleted)return RequestResponse<ProgramSelectionPreviewDto>
                    .Fail("Please complete your fitness profile first.", 400);

            // we gt the warinig only here because we want to show the user if the program is compatible or not, and if not, what are the reasons.
            var (_, warnings) = ProgramRecommendationRules.Calculate(programEntity, profile);

            var isPremium = program.AccessTier == AccessTier.Premium;
            var canSelect = program.Status == ProgramStatus.Published && !isPremium;

            return RequestResponse<ProgramSelectionPreviewDto>.Ok(
                new ProgramSelectionPreviewDto(
                    program.ProgramId,
                    program.Title,
                    IsCompatible: warnings.Count == 0,
                    Warnings: warnings,
                    IsPremium: isPremium,
                    CanSelect: canSelect));
        }
    }
}
