using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.Common.IsProgramInDraftStatus.Queries
{
    public record IsProgramInDraftStatusQuery(Guid ProgramId) : IRequest<RequestResponse<bool>>;

}
