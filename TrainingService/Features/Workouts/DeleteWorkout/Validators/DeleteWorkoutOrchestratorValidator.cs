using FluentValidation;
using TrainingService.Features.Workouts.DeleteWorkout.Orchestrator;

namespace TrainingService.Features.Workouts.DeleteWorkout.Validators
{
    public class DeleteWorkoutOrchestratorValidator : AbstractValidator<DeleteWorkoutOrchestrator>
    {
        public DeleteWorkoutOrchestratorValidator()
        {
            RuleFor(x => x.WorkoutId)
                .NotEmpty().WithMessage("Workout ID is required.");
        }
    }
}
