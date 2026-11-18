using FluentValidation;
using TrainingService.Features.Workouts.DeleteWorkout.Commands;

namespace TrainingService.Features.Workouts.DeleteWorkout.Validators
{
    public class DeleteWorkoutCommandValidator : AbstractValidator<DeleteWorkoutCommand>
    {
        public DeleteWorkoutCommandValidator()
        {
            RuleFor(x => x.WorkoutId)
                .NotEmpty().WithMessage("Workout ID is required.");
        }
    }
}
