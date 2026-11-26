namespace TrainingService.Features.TrainingPrograms.PreviewProgramSelection.Dtos
{
    public record ProgramSelectionPreviewDto(
        Guid ProgramId,
        string Title,
        bool IsCompatible,
        List<string> Warnings,
        bool IsPremium,
        bool CanSelect);
}
