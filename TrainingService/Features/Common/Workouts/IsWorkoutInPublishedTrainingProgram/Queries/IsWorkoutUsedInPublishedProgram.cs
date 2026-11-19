using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries
{
    public record IsWorkoutUsedInPublishedProgramQuery(Guid WorkoutId) : IRequest<RequestResponse<bool>>;

}
