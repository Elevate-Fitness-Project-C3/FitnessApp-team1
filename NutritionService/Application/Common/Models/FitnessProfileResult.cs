namespace NutritionService.Application.Common.Models;

/// <summary>
/// Result of requesting TDEE and profile status from Fitness Profile Service.
/// </summary>
public record FitnessProfileResult
{
    public bool IsSuccess { get; init; }
    public double? Tdee { get; init; }
    public bool IsProfileComplete { get; init; } = true;
    public bool IsOutdated { get; init; }
    public string? Message { get; init; }

    public static FitnessProfileResult Success(double tdee) =>
        new()
        {
            IsSuccess = true,
            Tdee = tdee,
            IsProfileComplete = true,
            IsOutdated = false
        };

    public static FitnessProfileResult Incomplete(string message = "Fitness profile is incomplete. Please complete your fitness profile to receive a calorie target.") =>
        new()
        {
            IsSuccess = false,
            IsProfileComplete = false,
            Message = message
        };

    public static FitnessProfileResult Outdated(string message = "Fitness profile is outdated. Please update your fitness profile to receive an accurate calorie target.") =>
        new()
        {
            IsSuccess = false,
            IsOutdated = true,
            Message = message
        };

    public static FitnessProfileResult Unavailable(string message = "Fitness Profile Service is currently unavailable. Please try again later or check that your fitness profile is up to date.") =>
        new()
        {
            IsSuccess = false,
            Message = message
        };

    public static FitnessProfileResult NotFound(string message = "Fitness profile not found. Please complete your fitness profile first.") =>
        new()
        {
            IsSuccess = false,
            IsProfileComplete = false,
            Message = message
        };
}
