using MediatR;
using TrainingService.Domain.Enums;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetAllWorkouts.Controllers.ViewModels;
using TrainingService.Features.Workouts.GetAllWorkouts.Queries;

namespace TrainingService.Features.Workouts.GetAllWorkouts.Controllers
{
    public static class GetAllWorkoutsEndpoint
    {

        public static void MapGetAllWorkoutsEndpoint(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/workouts", async (string? title,ExperienceLevel? difficulty,int pageIndex ,int pageSize , IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Send(new GetAllWorkoutsQuery(pageIndex, pageSize, title, difficulty), cancellationToken);
                if (!result.Success || result.Data == null)
                {
                    var failResponse = EndpointResponse<GetAllWorkoutsVM>.Fail(result.Message, result.StatusCode, result.Errors);
                    return Results.Json(failResponse, statusCode: failResponse.StatusCode);
                }

                var vm = new GetAllWorkoutsVM(
                 result.Data.Items.Select(w => new WorkoutItemVM(
                     w.Id,
                     w.Title,
                     w.Description,
                     w.Difficulty,
                     w.EstimatedDurationMinutes
                 )).ToList(),
                 result.Data.PageNumber,
                 result.Data.PageSize,
                 result.Data.TotalCount,
                 result.Data.TotalPages,
                 result.Data.HasPreviousPage,
                 result.Data.HasNextPage
             );

                var response = EndpointResponse<GetAllWorkoutsVM>.Ok(vm, result.Message);
                return Results.Json(response, statusCode: response.StatusCode);
            })
            .WithTags("Workouts")
            .WithName("GetAllWorkouts");
        }
    }
}
