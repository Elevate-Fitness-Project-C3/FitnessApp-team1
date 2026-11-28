using FluentValidation;
using TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Orchestrator;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Validators
{
    public class SelectTrainingProgramCommandValidator: AbstractValidator<SelectTrainingProgramOrchestrator>
    {
        public SelectTrainingProgramCommandValidator()
        {
            RuleFor(x => x.ProgramId)
                .NotEmpty()
                .WithMessage("ProgramId is required.")
                .NotEqual(Guid.Empty)
                .WithMessage("ProgramId cannot be an empty GUID.");

        }
    }
}
