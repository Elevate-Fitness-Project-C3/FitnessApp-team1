using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Orchestrators;
using TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Queries;
using TrainingService.Features.TrainingProgramFeature.GetTrainingProgramById.Queries;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Handlers
{
    public class BrowseTrainingProgramDetialsOrchestratorHandler:IRequestHandler<BrowseTrainingProgramDetialsOrchestrator, RequestResponse<TrainingProgramDetailsDto>>
    {
        private readonly IMediator _mediator;
        public BrowseTrainingProgramDetialsOrchestratorHandler(IMediator mediator) =>
            _mediator = mediator;

        public async Task<RequestResponse<TrainingProgramDetailsDto>> Handle(BrowseTrainingProgramDetialsOrchestrator request , CancellationToken ct)
        {
            var trainingProgram =await _mediator.Send(new GetTrainingProgramDetainlsQuery(request.TrainingProgramId),ct);

            if (trainingProgram.Data == null)
                return RequestResponse<TrainingProgramDetailsDto>.Fail(trainingProgram.Message, trainingProgram.StatusCode);

            var trainingProgramWorkoutSlots = await _mediator.Send(new GetTrainingProgramWoekoutSlotsOrchestrator(request.TrainingProgramId));

            return RequestResponse<TrainingProgramDetailsDto>.Fail("can't get detials.");
        }
    }
}
