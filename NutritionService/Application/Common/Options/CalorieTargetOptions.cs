namespace NutritionService.Application.Common.Options;

/// <summary>
/// Configuration options for calculating daily calorie targets based on user goals and TDEE.
/// Policy Rules:
/// - Maintain: targets baseline TDEE (adjustment = 0).
/// - GradualLoss: applies conservative deficit (default: -500 kcal/day).
/// - GradualGain: applies conservative surplus (default: +300 kcal/day).
/// - MinimumCalorieFloor: health safety floor (default: 1200 kcal/day) to prevent unsafe extreme deficits.
/// </summary>
public class CalorieTargetOptions
{
    public const string SectionName = "CalorieTarget";

    /// <summary>
    /// Calorie adjustment for GradualLoss goal in kcal/day (default: -500).
    /// </summary>
    public int LossAdjustment { get; set; } = -500;

    /// <summary>
    /// Calorie adjustment for GradualGain goal in kcal/day (default: +300).
    /// </summary>
    public int GainAdjustment { get; set; } = 300;

    /// <summary>
    /// Calorie adjustment for Maintain goal in kcal/day (default: 0).
    /// </summary>
    public int MaintainAdjustment { get; set; } = 0;

    /// <summary>
    /// Health safety floor in kcal/day below which targets will not be set (default: 1200).
    /// </summary>
    public int MinimumCalorieFloor { get; set; } = 1200;

    /// <summary>
    /// Optional positive deficit magnitude (e.g. 500). If specified, overrides LossAdjustment with -abs(val).
    /// </summary>
    public int? DeficitCalories { get; set; }

    /// <summary>
    /// Optional positive surplus magnitude (e.g. 300). If specified, overrides GainAdjustment with +abs(val).
    /// </summary>
    public int? SurplusCalories { get; set; }

    /// <summary>
    /// Evaluates effective deficit adjustment.
    /// </summary>
    public int GetLossAdjustment() => DeficitCalories.HasValue ? -Math.Abs(DeficitCalories.Value) : LossAdjustment;

    /// <summary>
    /// Evaluates effective surplus adjustment.
    /// </summary>
    public int GetGainAdjustment() => SurplusCalories.HasValue ? Math.Abs(SurplusCalories.Value) : GainAdjustment;
}
