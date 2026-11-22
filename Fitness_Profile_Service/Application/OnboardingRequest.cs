using FitnessProfileService.Domain.Enums;

namespace FitnessProfileService.Application;

public sealed record OnboardingRequest(
    Gender Gender,
    DateOnly DateOfBirth,
    FitnessGoal FitnessGoal,
    PhysicalActivityLevel PhysicalActivityLevel,
    decimal HeightCm,
    decimal WeightKg,
    string? TrainingExperienceLevel = null,
    byte? AvailableDaysPerWeek = null);

public sealed record MeasurementRequest(decimal HeightCm, decimal WeightKg);
