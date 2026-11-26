using TrainingService.Domain.Enums;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Dtos
{
    public record ProgramScoringDetailsDto(
        Guid ProgramId,
        string Title,
        FitnessGoal Goal,
        ExperienceLevel Level,
        int DaysPerWeek,
        ProgramStatus Status,
        AccessTier AccessTier);
}
