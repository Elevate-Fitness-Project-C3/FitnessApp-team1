namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos
{
    public record ProgramRecommendationDto(
        Guid ProgramId,
        string Title,
        int MatchScore,
        List<string> Warnings);
}
