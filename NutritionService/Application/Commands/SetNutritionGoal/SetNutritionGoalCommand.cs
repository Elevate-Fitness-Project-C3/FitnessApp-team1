using MediatR;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Commands.SetNutritionGoal;

public record SetNutritionGoalCommand(NutritionGoal Goal) : IRequest<NutritionPreferenceResponse>;
