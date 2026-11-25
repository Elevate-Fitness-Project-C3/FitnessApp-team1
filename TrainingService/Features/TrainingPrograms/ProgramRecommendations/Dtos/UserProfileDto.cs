namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos
{
    public  record UserProfileDto(
        Guid UserId,
        string FitnessGoal,
        string? TrainingExperienceLevel,
        byte? AvailableDaysPerWeek,
        bool IsOnboardingCompleted);
}
