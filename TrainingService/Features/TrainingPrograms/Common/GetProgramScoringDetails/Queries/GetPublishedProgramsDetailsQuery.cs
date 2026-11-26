using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Dtos;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries
{
    public record GetPublishedProgramsDetailsQuery : IRequest<RequestResponse<List<ProgramScoringDetailsDto>>>;
}
