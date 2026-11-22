using MediatR;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Commands.UpdateMeal;

public record UpdateMealCommand(
    Guid Id,
    string Name,
    MealType Type,
    int Calories,
    double ProteinGrams
) : IRequest<bool>;
