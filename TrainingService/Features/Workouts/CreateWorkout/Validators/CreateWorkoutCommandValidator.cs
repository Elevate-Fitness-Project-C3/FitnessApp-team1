using FluentValidation;
using TrainingService.Features.Workouts.CreateWorkout.Commands;

namespace TrainingService.Features.Workouts.CreateWorkout.Validators
{
    public class CreateWorkoutCommandValidator : AbstractValidator<CreateWorkoutCommand>
    {
        public CreateWorkoutCommandValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.EstimatedDurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than zero.");
        }
    }
}
