using FitnessProfileService.Domain.Enums;

namespace FitnessProfileService.Application;

public sealed record BodyMeasurementResponse(
    long MeasurementId,
    decimal HeightCm,
    decimal WeightKg,
    DateTime RecordedAtUtc);

public sealed record ProfileResponse(
    Guid UserId,
    int Age,
    Gender Gender,
    DateOnly DateOfBirth,
    FitnessGoal FitnessGoal,
    PhysicalActivityLevel PhysicalActivityLevel,
    string? TrainingExperienceLevel,
    byte? AvailableDaysPerWeek,
    bool IsOnboardingCompleted,
    BodyMeasurementResponse? LatestMeasurement);

public enum ProfileChangeStatus
{
    Created,
    Conflict
}

public sealed record ProfileChangeResult(
    ProfileChangeStatus Status,
    ProfileResponse? Profile,
    string? Code,
    string? Detail)
{
    public static ProfileChangeResult Created(ProfileResponse profile)
        => new(ProfileChangeStatus.Created, profile, null, null);

    public static ProfileChangeResult Conflict()
        => new(
            ProfileChangeStatus.Conflict,
            null,
            "onboarding_already_completed",
            "This user already has a fitness profile.");
}
