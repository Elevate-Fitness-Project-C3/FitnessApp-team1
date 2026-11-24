using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Queries
{
    public record GetTrainingProgramWorkoutSlotsQuery(Guid ProgramId, int PageNumber, int PageSize) : IRequest<RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>;
}
