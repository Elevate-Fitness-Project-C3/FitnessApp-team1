using MediatR;
using TrainingService.DTOs;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.GetTrainingProgramById.Queries
{
    public record GetTrainingProgramByIdQuery(Guid ProgramId) : IRequest<RequestResponse<TrainingProgramDto>>;
}
