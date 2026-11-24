using NutritionService.Domain.Enums;

namespace NutritionService.Application.DTOs;

/// <summary>
/// Daily calorie target calculation response.
/// </summary>
public record DailyCalorieTargetResponse(
    bool IsAvailable,
    int? TargetCalories,
    int? BaselineTdee,
    int? Adjustment,
    NutritionGoal? Goal,
    string? Message,
    string Disclaimer)
{
    public const string GeneralEstimateDisclaimer = "This target is a general estimate, not a personalized medical or clinical nutrition prescription.";

    public static DailyCalorieTargetResponse Available(
        int targetCalories,
        int baselineTdee,
        int adjustment,
        NutritionGoal goal,
        string? message = null) =>
        new(
            IsAvailable: true,
            TargetCalories: targetCalories,
            BaselineTdee: baselineTdee,
            Adjustment: adjustment,
            Goal: goal,
            Message: message,
            Disclaimer: GeneralEstimateDisclaimer
        );

    public static DailyCalorieTargetResponse Unavailable(
        NutritionGoal? goal,
        string message) =>
        new(
            IsAvailable: false,
            TargetCalories: null,
            BaselineTdee: null,
            Adjustment: null,
            Goal: goal,
            Message: message,
            Disclaimer: GeneralEstimateDisclaimer
        );
}
