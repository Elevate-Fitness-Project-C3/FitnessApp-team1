using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.WorkoutExercises.IsWorkoutExerciseInPublishedProgram.Queries
{
    public record IsWorkoutExerciseInPublishedProgramQuery(Guid WorkoutExerciseId): IRequest<RequestResponse<bool>>;
}
