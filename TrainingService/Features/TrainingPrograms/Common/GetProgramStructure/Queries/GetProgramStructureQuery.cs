using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Dtos;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Queries
{
    public record GetProgramStructureQuery(Guid ProgramId): IRequest<RequestResponse<ProgramStructureDto>>;
}
