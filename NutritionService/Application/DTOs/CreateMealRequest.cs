using NutritionService.Domain.Enums;

namespace NutritionService.Application.DTOs;

public record CreateMealRequest(
    string Name,
    MealType Type,
    int Calories,
    double ProteinGrams
);
