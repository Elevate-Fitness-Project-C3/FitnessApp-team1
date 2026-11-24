using MediatR;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Commands.CreateMeal;

public record CreateMealCommand(
    string Name,
    MealType Type,
    int Calories,
    double ProteinGrams
) : IRequest<Guid>;
