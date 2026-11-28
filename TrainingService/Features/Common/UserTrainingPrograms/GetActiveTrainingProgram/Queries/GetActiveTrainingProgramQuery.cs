using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.UserTrainingPrograms.GetActiveTrainingProgram.Queries
{
    public record GetActiveTrainingProgramQuery(Guid UserId): IRequest<RequestResponse<bool>>;
}
