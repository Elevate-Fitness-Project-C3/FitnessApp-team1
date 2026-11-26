using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Dtos;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries
{
    public record GetPublishedProgramsScoringDetailsQuery: IRequest<RequestResponse<List<ProgramScoringDetailsDto>>>;
}
