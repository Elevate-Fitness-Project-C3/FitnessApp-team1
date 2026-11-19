using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries
{
    public record IsWorkoutUsedInPublishedProgramQuery(Guid WorkoutId = default,Guid? WorkoutExerciseId = null) : IRequest<RequestResponse<bool>>;

}
