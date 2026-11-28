using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Commands
{
    public record StopActiveTrainingProgramCommand(Guid UserId): IRequest<RequestResponse<Unit>>;
}
