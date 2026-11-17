using MediatR;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetWorkoutById.Controllers.ViewModels;
using TrainingService.Features.Workouts.GetWorkoutById.Queries;

namespace TrainingService.Features.Workouts.GetWorkoutById.Controllers
{
    public static class GetWorkoutByIdEndpoint
    {
        public static void MapGetWorkoutByIdEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/workouts/{id:guid}", async (Guid id, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetWorkoutByIdQuery(id), cancellationToken);

                if (!result.Success || result.Data == null)
                {
                    var failResponse = EndpointResponse<GetWorkoutByIdVM>.Fail(result.Message, result.StatusCode, result.Errors);
                    return Results.Json(failResponse, statusCode: failResponse.StatusCode);
                }

                var vm = new GetWorkoutByIdVM(
                    result.Data.Id,
                    result.Data.Title,
                    result.Data.Description,
                    result.Data.Difficulty,
                    result.Data.EstimatedDurationMinutes
                );

                var response = EndpointResponse<GetWorkoutByIdVM>.Ok(vm, result.Message);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("GetWorkoutById");
        }
    }
}
