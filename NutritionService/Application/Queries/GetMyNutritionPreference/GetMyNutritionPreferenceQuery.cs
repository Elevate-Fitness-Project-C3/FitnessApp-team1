using MediatR;
using NutritionService.Application.DTOs;

namespace NutritionService.Application.Queries.GetMyNutritionPreference;

public record GetMyNutritionPreferenceQuery() : IRequest<NutritionPreferenceResponse>;
