using FluentValidation;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Validators
{
    public class PublishTrainingProgramCommandValidator :AbstractValidator<PublishTrainingProgramCommand>
    {
        public PublishTrainingProgramCommandValidator()
        {
            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("Program Id is required.");
        }
    }
}
