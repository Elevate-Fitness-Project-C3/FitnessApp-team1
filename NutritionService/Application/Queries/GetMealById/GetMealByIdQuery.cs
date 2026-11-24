using MediatR;
using NutritionService.Application.DTOs;

namespace NutritionService.Application.Queries.GetMealById;

public record GetMealByIdQuery(Guid Id) : IRequest<MealDto?>;
