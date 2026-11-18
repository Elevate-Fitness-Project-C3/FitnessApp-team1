using FluentValidation;
using TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Commands;

namespace TrainingService.Features.WorkoutExercises.CreateWorkoutExercise.Validators
{
    public class CreateExerciseToWorkoutCommandValidator : AbstractValidator<AddExerciseToWorkoutCommand>
    {
        public CreateExerciseToWorkoutCommandValidator()
        {
            RuleFor(x => x.WorkoutId)
                .NotEmpty().WithMessage("Workout ID is required.");

            RuleFor(x => x.ExerciseId)
                .NotEmpty().WithMessage("Exercise ID is required.");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater.");
        }
    }
}
