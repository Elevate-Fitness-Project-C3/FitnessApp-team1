using FitnessApp.Auth;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using NutritionService.Application.Commands.CreateMeal;
using NutritionService.Application.Commands.DeactivateMeal;
using NutritionService.Application.Commands.UpdateMeal;
using NutritionService.Application.DTOs;
using NutritionService.Application.Queries.BrowseMeals;
using NutritionService.Application.Queries.GetMealById;

namespace NutritionService.API.Endpoints;

public static class MealEndpoints
{
    public static IEndpointRouteBuilder MapMealEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/nutrition/meals")
            .WithTags("Meals");

        group.MapGet("/", async ([AsParameters] BrowseMealsQuery query, ISender mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(query, cancellationToken);
            return Results.Ok(response);
        })
        .WithName("BrowseMeals")
        .WithSummary("Browse active meals with optional search and type filtering");

        group.MapGet("/{id:guid}", async (Guid id, ISender mediator, CancellationToken cancellationToken) =>
        {
            var response = await mediator.Send(new GetMealByIdQuery(id), cancellationToken);
            return response is not null
                ? Results.Ok(response)
                : Results.NotFound(new { message = $"Meal with id '{id}' was not found." });
        })
        .WithName("GetMealById")
        .WithSummary("Get meal details by ID");

        group.MapPost("/", async (CreateMealRequest request, IValidator<CreateMealCommand> validator, ISender mediator, CancellationToken cancellationToken) =>
        {
            var command = new CreateMealCommand(request.Name, request.Type, request.Calories, request.ProteinGrams);
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var id = await mediator.Send(command, cancellationToken);
            return Results.Created($"/api/nutrition/meals/{id}", new { id });
        })
        .RequireAdmin()
        .WithName("CreateMeal")
        .WithSummary("Create a new meal (Admin only)");

        group.MapPut("/{id:guid}", async (Guid id, UpdateMealRequest request, IValidator<UpdateMealCommand> validator, ISender mediator, CancellationToken cancellationToken) =>
        {
            var command = new UpdateMealCommand(id, request.Name, request.Type, request.Calories, request.ProteinGrams);
            var validationResult = await validator.ValidateAsync(command, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }

            var success = await mediator.Send(command, cancellationToken);
            return success
                ? Results.Ok(new { success = true, message = "Meal updated successfully." })
                : Results.NotFound(new { message = $"Meal with id '{id}' was not found." });
        })
        .RequireAdmin()
        .WithName("UpdateMeal")
        .WithSummary("Update an existing meal (Admin only)");

        group.MapDelete("/{id:guid}", async (Guid id, ISender mediator, CancellationToken cancellationToken) =>
        {
            var success = await mediator.Send(new DeactivateMealCommand(id), cancellationToken);
            return success
                ? Results.NoContent()
                : Results.NotFound(new { message = $"Meal with id '{id}' was not found." });
        })
        .RequireAdmin()
        .WithName("DeactivateMeal")
        .WithSummary("Deactivate a meal (Admin only)");

        return app;
    }
}
