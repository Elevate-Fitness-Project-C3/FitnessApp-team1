using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.Common.IsProgramInPublishedStatus.Queries
{
    public record IsProgramInPublishedStatusQuery(Guid ProgramId) : IRequest<RequestResponse<bool>>;

}
