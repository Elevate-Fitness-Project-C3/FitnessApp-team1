using MediatR;
using NutritionService.Application.DTOs;

namespace NutritionService.Application.Queries.GetDailyCalorieTarget;

/// <summary>
/// Query to calculate and retrieve daily calorie target based on fresh TDEE and goal preference.
/// </summary>
public record GetDailyCalorieTargetQuery : IRequest<DailyCalorieTargetResponse>;
