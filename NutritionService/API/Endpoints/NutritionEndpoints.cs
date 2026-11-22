using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NutritionService.Application.Commands.SetNutritionGoal;
using NutritionService.Application.DTOs;
using NutritionService.Application.Queries.GetMyNutritionPreference;

namespace NutritionService.API.Endpoints;

public static class NutritionEndpoints
{
    public static IEndpointRouteBuilder MapNutritionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/nutrition/preferences")
            .WithTags("Nutrition Preferences")
            .RequireAuthorization();

        group.MapGet("/me", async (ISender mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetMyNutritionPreferenceQuery(), cancellationToken);
            return Results.Ok(response);
        })
        .WithName("GetMyNutritionPreference")
        .WithSummary("Get the current user's nutrition goal preference");

        group.MapPut("/me", async (SetNutritionGoalRequest request, IValidator<SetNutritionGoalCommand> validator, ISender mediator, CancellationToken cancellationToken) =>
        {
            var command = new SetNutritionGoalCommand(request.Goal);
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var success = await mediator.Send(command, cancellationToken);
            return Results.Ok(new { success, message = "Nutrition goal preference updated successfully." });
        })
        .WithName("SetMyNutritionPreference")
        .WithSummary("Set or update the current user's nutrition goal preference");

        group.MapPost("/", async (SetNutritionGoalRequest request, IValidator<SetNutritionGoalCommand> validator, ISender mediator, CancellationToken cancellationToken) =>
        {
            var command = new SetNutritionGoalCommand(request.Goal);
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var success = await mediator.Send(command, cancellationToken);
            return Results.Ok(new { success, message = "Nutrition goal preference updated successfully." });
        })
        .WithName("CreateOrUpdateNutritionPreference")
        .WithSummary("Create or update the current user's nutrition goal preference");

        return app;
    }
}
