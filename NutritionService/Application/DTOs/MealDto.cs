using NutritionService.Domain.Enums;

namespace NutritionService.Application.DTOs;

public record MealDto(
    Guid Id,
    string Name,
    MealType Type,
    int Calories,
    double ProteinGrams,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
