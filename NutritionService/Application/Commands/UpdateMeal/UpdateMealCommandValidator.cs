using FluentValidation;

namespace NutritionService.Application.Commands.UpdateMeal;

public class UpdateMealCommandValidator : AbstractValidator<UpdateMealCommand>
{
    public UpdateMealCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type must be a valid meal type (Breakfast, Lunch, Dinner, Snack).");

        RuleFor(x => x.Calories)
            .GreaterThanOrEqualTo(0).WithMessage("Calories must be non-negative.");

        RuleFor(x => x.ProteinGrams)
            .GreaterThanOrEqualTo(0).WithMessage("Protein must be non-negative.");
    }
}
