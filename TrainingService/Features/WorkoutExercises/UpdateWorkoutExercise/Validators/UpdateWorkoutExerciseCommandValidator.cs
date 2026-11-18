using FluentValidation;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Commands;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Validators
{
    public class UpdateWorkoutExerciseCommandValidator : AbstractValidator<UpdateWorkoutExerciseCommand>
    {
        public UpdateWorkoutExerciseCommandValidator()
        {
            RuleFor(x => x.WorkoutExerciseId)
                .NotEmpty().WithMessage("WorkoutExercise ID is required.");

            RuleFor(x => x.OrderIndex)
                .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater.");
        }
    }
}
