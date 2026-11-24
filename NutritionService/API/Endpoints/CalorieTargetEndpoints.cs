using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NutritionService.Application.DTOs;
using NutritionService.Application.Queries.GetDailyCalorieTarget;

namespace NutritionService.API.Endpoints;

public static class CalorieTargetEndpoints
{
    public static IEndpointRouteBuilder MapCalorieTargetEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/nutrition/targets")
            .WithTags("Nutrition Targets")
            .RequireAuthorization();

        group.MapGet("/daily", async (ISender mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetDailyCalorieTargetQuery(), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetDailyCalorieTarget")
        .WithSummary("Get the calculated daily calorie target based on fresh TDEE and nutrition goal")
        .Produces<DailyCalorieTargetResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }
}
