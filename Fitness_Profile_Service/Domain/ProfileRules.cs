namespace FitnessProfileService.Domain;

public static class ProfileRules
{
    public const int MinAgeYears = 15;
    public const int MaxAgeYears = 100;

    public const decimal MinHeightCm = 100m;
    public const decimal MaxHeightCm = 250m;

    public const decimal MinWeightKg = 30m;
    public const decimal MaxWeightKg = 300m;

    public const int MaxDecimals = 1;
    public const int MaxTrainingExperienceLength = 30;
    public const byte MinAvailableDaysPerWeek = 1;
    public const byte MaxAvailableDaysPerWeek = 7;

    // Kept for compatibility with the original skeleton spelling.
    public const int MacDecimals = MaxDecimals;
    public const decimal MinweightKg = MinWeightKg;
    public const decimal MaxweightKg = MaxWeightKg;
}
