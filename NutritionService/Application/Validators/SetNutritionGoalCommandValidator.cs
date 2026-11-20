using FluentValidation;
using NutritionService.Application.Commands.SetNutritionGoal;

namespace NutritionService.Application.Validators;

public class SetNutritionGoalCommandValidator : AbstractValidator<SetNutritionGoalCommand>
{
    public SetNutritionGoalCommandValidator()
    {
        RuleFor(x => x.Goal)
            .IsInEnum()
            .WithMessage("Goal must be a valid nutrition goal: Maintain (1), GradualLoss (2), or GradualGain (3).");
    }
}
