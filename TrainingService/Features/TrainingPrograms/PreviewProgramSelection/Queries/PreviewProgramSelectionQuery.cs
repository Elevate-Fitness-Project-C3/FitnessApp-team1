using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Dtos;

namespace TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Queries
{
    public record PreviewProgramSelectionQuery(Guid ProgramId): IRequest<RequestResponse<ProgramSelectionPreviewDto>>;
}
