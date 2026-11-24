using FitnessProfileService.Domain.Enums;

namespace FitnessProfileService.Domain;

public class FitnessProfile : BaseEntity
{
    private FitnessProfile() { }

    public Guid UserId { get; private set; }
    public Gender Gender { get; private set; }
    public DateOnly DateOfBirth { get; private set; }
    public FitnessGoal PrimaryFitnessGoal { get; private set; }
    public PhysicalActivityLevel PhysicalActivityLevel { get; private set; }
    public string? TrainingExperienceLevel { get; private set; }
    public byte? AvailableDaysPerWeek { get; private set; }
    public bool IsOnboardingCompleted { get; private set; }

    public static FitnessProfile Create(
        Guid userId,
        Gender gender,
        DateOnly dateOfBirth,
        FitnessGoal goal,
        PhysicalActivityLevel activityLevel,
        DateTime nowUtc,
        string? trainingExperienceLevel = null,
        byte? availableDaysPerWeek = null) => new()
        {
            Id = Guid.NewGuid(),
            CreatedAt = nowUtc,
            UserId = userId,
            Gender = gender,
            DateOfBirth = dateOfBirth,
            PrimaryFitnessGoal = goal,
            PhysicalActivityLevel = activityLevel,
            TrainingExperienceLevel = string.IsNullOrWhiteSpace(trainingExperienceLevel)
                ? null
                : trainingExperienceLevel.Trim(),
            AvailableDaysPerWeek = availableDaysPerWeek
        };

    public void RefreshOnboardingStatus(bool hasInitialMeasurement, DateTime nowUtc)
    {
        IsOnboardingCompleted =
            hasInitialMeasurement
            && DateOfBirth != default
            && Enum.IsDefined(Gender)
            && Enum.IsDefined(PrimaryFitnessGoal)
            && Enum.IsDefined(PhysicalActivityLevel);

        UpdatedAt = nowUtc;
    }
}
