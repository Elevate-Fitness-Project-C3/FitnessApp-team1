using FluentValidation;
using TrainingService.Features.Workouts.UpdateWorkout.Orchestrator;

namespace TrainingService.Features.Workouts.UpdateWorkout.Validators
{
    public class UpdateWorkoutCommandValidator  : AbstractValidator<UpdateWorkoutOrchestrator>
    {
        public UpdateWorkoutCommandValidator()
        {
            RuleFor(x => x.WorkoutId)
                .NotEmpty().WithMessage("Workout ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

            RuleFor(x => x.EstimatedDurationMinutes)
                .GreaterThan(0).WithMessage("Duration must be greater than zero.");
        }
    }
}
