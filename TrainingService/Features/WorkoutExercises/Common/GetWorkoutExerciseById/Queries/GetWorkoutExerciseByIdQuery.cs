using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Features.Shared;

namespace TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries
{
    public record GetWorkoutExerciseByIdQuery(Guid WorkoutExerciseId) : IRequest<RequestResponse<WorkoutExercise>>;
}
