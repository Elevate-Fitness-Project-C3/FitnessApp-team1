using MediatR;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Queries.BrowseMeals;

public record BrowseMealsQuery(
    string? Search = null,
    MealType? Type = null,
    int Page = 1,
    int PageSize = 20
) : IRequest<PaginatedMealsResponse>;
