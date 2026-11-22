namespace NutritionService.Application.DTOs;

public record PaginatedMealsResponse(
    IReadOnlyList<MealDto> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);
