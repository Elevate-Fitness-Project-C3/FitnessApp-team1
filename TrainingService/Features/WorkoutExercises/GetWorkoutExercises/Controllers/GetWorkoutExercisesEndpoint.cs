using MediatR;
using Microsoft.AspNetCore.Mvc;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Controllers.ViewModels;
using TrainingService.Features.WorkoutExercises.GetWorkoutExercises.Queries;

namespace TrainingService.Features.Workouts.GetWorkoutExercises.Controllers;

public static class GetWorkoutExercisesEndpoint
{
    public static void MapGetWorkoutExercisesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/workouts/{workoutId:guid}/exercises", async ([FromRoute] Guid workoutId, IMediator mediator, CancellationToken ct) =>
        {
            var result = await mediator.Send(new GetWorkoutExercisesQuery(workoutId), ct);
            if (!result.Success || result.Data == null)
            {
                var failResponse = EndpointResponse<GetWorkoutExercisesVM>.Fail(result.Message, result.StatusCode, result.Errors);
                return Results.Json(failResponse, statusCode: failResponse.StatusCode);
            }

            var vm = new GetWorkoutExercisesVM(
                result.Data.WorkoutId,
                result.Data.WorkoutTitle,
                result.Data.Exercises.Select(e => new WorkoutExerciseItemVM(
                    e.ExerciseId,
                    e.ExerciseName,
                    e.OrderIndex,
                    e.TargetSets,
                    e.TargetReps,
                    e.TargetDurationSeconds
                )).ToList()
            );

            var response = EndpointResponse<GetWorkoutExercisesVM>.Ok(vm, result.Message);
            return Results.Json(response, statusCode: response.StatusCode);
        })
        .WithTags("Workouts")
        .WithName("GetWorkoutExercises");
    }
}