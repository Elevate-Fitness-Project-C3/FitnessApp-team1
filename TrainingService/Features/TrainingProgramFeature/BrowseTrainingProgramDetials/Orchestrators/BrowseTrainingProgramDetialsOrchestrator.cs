using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Orchestrators
{
    public record BrowseTrainingProgramDetialsOrchestrator(Guid TrainingProgramId) : IRequest<RequestResponse<TrainingProgramDetailsDto>>;
}
