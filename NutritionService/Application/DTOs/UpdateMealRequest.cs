using NutritionService.Domain.Enums;

namespace NutritionService.Application.DTOs;

public record UpdateMealRequest(
    string Name,
    MealType Type,
    int Calories,
    double ProteinGrams
);
