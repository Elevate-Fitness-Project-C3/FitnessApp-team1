using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Commands
{
    public record PublishTrainingProgramCommand(Guid ProgramId) : IRequest<RequestResponse<Unit>>;

}
