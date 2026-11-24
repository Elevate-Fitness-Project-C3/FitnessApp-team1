using TrainingService.Domain.Enums;

namespace TrainingService.Features.SetTrainingRefrence.DTO
{
    public record TrainingPreferencesDto
    (
        ExperienceLevel TrainingExperienceLevel,
    int AvailableDaysPerWeek
    );
}
