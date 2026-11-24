using MediatR;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Queries
{
    public record GetTrainingProgramDetainlsQuery(Guid programId) : IRequest<RequestResponse<TrainingProgramDetailsDto>>;
}
