using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.ProgramWorkoutSlotFeature.Orchestrators;
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

        public async Task<RequestResponse<TrainingProgramDetailsDto>> Handle(
            BrowseTrainingProgramDetialsOrchestrator request,
            CancellationToken ct)
        {
            var program = await _mediator.Send(
                new GetTrainingProgramDetainlsQuery(request.TrainingProgramId), ct);

            if (!program.Success)
                return RequestResponse<TrainingProgramDetailsDto>
                    .Fail(program.Message, program.StatusCode);
            

            if (program.Data == null)
                return RequestResponse<TrainingProgramDetailsDto>
                    .Fail("Program data is missing.", 500);

            var slots = await _mediator.Send(new GetTrainingProgramWorkoutSlotsOrchestrator
                (request.TrainingProgramId, request.PageNumber, request.PageSize), ct);

            if (!slots.Success)
                return RequestResponse<TrainingProgramDetailsDto>
                    .Fail(slots.Message, slots.StatusCode);

            if (slots.Data == null)
                return RequestResponse<TrainingProgramDetailsDto>
                    .Fail("Workout slots data is missing.", 500);

            program.Data.WorkoutSlots = slots.Data;

            return RequestResponse<TrainingProgramDetailsDto>.Ok(program.Data);
        }
    }
}
