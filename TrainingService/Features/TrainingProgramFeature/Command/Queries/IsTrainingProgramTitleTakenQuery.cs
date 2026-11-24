using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.Command.Queries
{
    public record IsTrainingProgramTitleTakenQuery(string Title) : IRequest<RequestResponse<bool>>;
    
}
