using NutritionService.Domain.Enums;

namespace NutritionService.Application.DTOs;

public record NutritionPreferenceResponse(
    bool HasPreference,
    NutritionGoal? Goal,
    DateTime? UpdatedAtUtc,
    string? Message
);
