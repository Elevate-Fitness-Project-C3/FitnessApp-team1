using FluentValidation;
using TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Orchestrator;

namespace TrainingService.Features.WorkoutExercises.UpdateWorkoutExercise.Validators
{
    public class UpdateWorkoutExerciseCommandValidator : AbstractValidator<UpdateWorkoutExerciseOrchestrator>
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
