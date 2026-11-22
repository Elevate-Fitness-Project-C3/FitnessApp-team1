using FitnessProfileService.Domain;

namespace FitnessProfileService.Application;

public sealed record ValidationResult(string? Code, string? Detail)
{
    public bool IsValid => Code is null;

    public static ValidationResult Ok() => new(null, null);

    public static ValidationResult Fail(string code, string detail) => new(code, detail);
}

public static class OnboardingValidator
{
    public static ValidationResult Validate(OnboardingRequest request, DateOnly todayUtc)
    {
        if (!Enum.IsDefined(request.Gender))
            return ValidationResult.Fail("gender_invalid", "Gender must be Male or Female.");

        if (!Enum.IsDefined(request.FitnessGoal))
            return ValidationResult.Fail("fitness_goal_invalid", "FitnessGoal is not valid.");

        if (!Enum.IsDefined(request.PhysicalActivityLevel))
            return ValidationResult.Fail("activity_level_invalid", "PhysicalActivityLevel is not valid.");

        if (request.DateOfBirth == default || request.DateOfBirth > todayUtc)
            return ValidationResult.Fail("date_of_birth_invalid", "DateOfBirth must be a valid past date.");

        var age = AgeCalculator.AgeOn(request.DateOfBirth, todayUtc);
        if (age < ProfileRules.MinAgeYears || age > ProfileRules.MaxAgeYears)
            return ValidationResult.Fail(
                "age_out_of_range",
                $"Age must be between {ProfileRules.MinAgeYears} and {ProfileRules.MaxAgeYears} years.");

        var measurement = ValidateMeasurement(request.HeightCm, request.WeightKg);
        if (!measurement.IsValid)
            return measurement;

        if (request.TrainingExperienceLevel is { Length: > ProfileRules.MaxTrainingExperienceLength })
        {
            return ValidationResult.Fail(
                "training_experience_too_long",
                $"TrainingExperienceLevel cannot exceed {ProfileRules.MaxTrainingExperienceLength} characters.");
        }

        if (request.AvailableDaysPerWeek is < ProfileRules.MinAvailableDaysPerWeek or > ProfileRules.MaxAvailableDaysPerWeek)
        {
            return ValidationResult.Fail(
                "available_days_out_of_range",
                $"AvailableDaysPerWeek must be between {ProfileRules.MinAvailableDaysPerWeek} and {ProfileRules.MaxAvailableDaysPerWeek}.");
        }

        return ValidationResult.Ok();
    }

    public static ValidationResult ValidateMeasurement(decimal heightCm, decimal weightKg)
    {
        if (heightCm < ProfileRules.MinHeightCm || heightCm > ProfileRules.MaxHeightCm)
            return ValidationResult.Fail(
                "height_out_of_range",
                $"HeightCm must be between {ProfileRules.MinHeightCm} and {ProfileRules.MaxHeightCm}.");

        if (weightKg < ProfileRules.MinWeightKg || weightKg > ProfileRules.MaxWeightKg)
            return ValidationResult.Fail(
                "weight_out_of_range",
                $"WeightKg must be between {ProfileRules.MinWeightKg} and {ProfileRules.MaxWeightKg}.");

        if (HasTooManyDecimals(heightCm) || HasTooManyDecimals(weightKg))
            return ValidationResult.Fail(
                "too_many_decimals",
                $"HeightCm and WeightKg allow at most {ProfileRules.MaxDecimals} decimal place(s).");

        return ValidationResult.Ok();
    }

    private static bool HasTooManyDecimals(decimal value)
        => decimal.Round(value, ProfileRules.MaxDecimals) != value;
}
