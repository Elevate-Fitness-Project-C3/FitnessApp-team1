using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.ProgramWorkoutSlotFeature.Orchestrators;
using TrainingService.Features.ProgramWorkoutSlotFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Handlers
{
    public class GetTrainingProgramWorkoutSlotsOrchestratorHandler : IRequestHandler<GetTrainingProgramWorkoutSlotsOrchestrator, RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>
    {
        private readonly IMediator _mediator;
        public GetTrainingProgramWorkoutSlotsOrchestratorHandler(IMediator mediator) =>
            _mediator = mediator;

        public async Task<RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>Handle(GetTrainingProgramWorkoutSlotsOrchestrator request,CancellationToken ct)
        {
            var slots = await _mediator.Send(
     new GetTrainingProgramWorkoutSlotsQuery(
         request.TrainingProgramId,
         request.PageNumber,
         request.PageSize), ct);

            if (!slots.Success)
                return RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>
                    .Fail(slots.Message, slots.StatusCode);
            if (slots.Data == null)
                return RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>
                    .Fail("No workout slots data returned.", 500);

            return RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>
                .Ok(slots.Data);
        }
    }
}
