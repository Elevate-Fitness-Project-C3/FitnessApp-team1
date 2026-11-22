using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Commands
{
    public record ArchiveTrainingProgramCommand(Guid ProgramId) : IRequest<RequestResponse<Unit>>;

}
