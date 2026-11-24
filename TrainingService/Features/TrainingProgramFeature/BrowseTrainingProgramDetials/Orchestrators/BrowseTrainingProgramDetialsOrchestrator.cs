using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Orchestrators
{
    public record BrowseTrainingProgramDetialsOrchestrator(Guid TrainingProgramId, int PageNumber=1, int PageSize1=10) : IRequest<RequestResponse<TrainingProgramDetailsDto>>;
}
