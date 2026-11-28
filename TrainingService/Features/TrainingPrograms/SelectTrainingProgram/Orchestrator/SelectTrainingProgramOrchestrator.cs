using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Orchestrator
{
    public record SelectTrainingProgramOrchestrator(Guid ProgramId, bool AcceptWarnings): IRequest<RequestResponse<Unit>>;
}
