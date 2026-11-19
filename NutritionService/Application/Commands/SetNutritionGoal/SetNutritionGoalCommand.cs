using MediatR;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Commands.SetNutritionGoal;

public record SetNutritionGoalCommand(NutritionGoal Goal) : IRequest<bool>;
