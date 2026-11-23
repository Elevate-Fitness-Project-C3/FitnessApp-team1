using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Dtos;

namespace TrainingService.Features.WorkoutExercises.Common.GetWorkoutExerciseById.Queries
{
    public record GetWorkoutExerciseByIdQuery(Guid WorkoutExerciseId) : IRequest<RequestResponse<WorkoutExerciseDto>>;
}
