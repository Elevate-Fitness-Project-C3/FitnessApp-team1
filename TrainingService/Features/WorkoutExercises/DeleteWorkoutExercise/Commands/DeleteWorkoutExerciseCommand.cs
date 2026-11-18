using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.DeleteWorkoutExercise.Commands
{
    public record DeleteWorkoutExerciseCommand(Guid WorkoutExerciseId) : IRequest<RequestResponse<Unit>>;
}
