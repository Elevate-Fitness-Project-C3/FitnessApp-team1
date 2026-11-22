using TrainingService.Domain.Enums;

namespace TrainingService.ViewModels.TrainingProgram
{
    public class BrowseTrainingProgramViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public FitnessGoal Goal { get; set; }
        public ExperienceLevel RequiredLevel { get; set; }
        public int DurationWeeks { get; set; }
        public int DaysPerWeek { get; set; }
        public AccessTier AccessTier { get; set; }
    }
}
