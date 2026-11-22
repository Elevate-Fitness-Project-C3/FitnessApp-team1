using FluentValidation;
using TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Validators
{
    public class ArchiveTrainingProgramCommandValidator : AbstractValidator<ArchiveTrainingProgramCommand>
    {
        public ArchiveTrainingProgramCommandValidator()
        {
            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("Program ID is required.");
        }
    }
}
