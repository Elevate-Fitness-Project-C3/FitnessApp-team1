using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Dtos;

namespace TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Queries
{
    public record GetWorkoutExercisesQuery(Guid WorkoutId) : IRequest<RequestResponse<WorkoutWithExercisesDto>>;
}
