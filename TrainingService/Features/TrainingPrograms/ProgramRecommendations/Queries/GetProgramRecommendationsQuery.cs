using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Queries
{
    public record GetProgramRecommendationsQuery : IRequest<RequestResponse<List<ProgramRecommendationDto>>>;
}
