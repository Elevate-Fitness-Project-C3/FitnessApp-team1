using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Orchestrators
{
    public record GetTrainingProgramWorkoutSlotsOrchestrator(Guid TrainingProgramId, int PageNumber, int PageSize) : IRequest<RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>;
}
