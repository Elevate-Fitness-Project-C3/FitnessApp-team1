using MediatR;
using TrainingService.DTOs;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.Queries
{
    public record GetTrainingProgramByIdQuery(Guid ProgramId) : IRequest<RequestResponse<TrainingProgramDto>>;
}
