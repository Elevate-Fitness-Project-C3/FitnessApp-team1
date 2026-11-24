using TrainingService.Domain.Enums;

namespace TrainingService.DTOs.BrowseTrainingProgramDetails
{
    public class TrainingProgramDetailsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public FitnessGoal Goal { get; set; }
        public ExperienceLevel RequiredLevel { get; set; }
        public int DurationWeeks { get; set; }
        public int DaysPerWeek { get; set; }
        public AccessTier AccessTier { get; set; }
        public bool IsLocked { get; set; }

        public List<ProgramWorkoutSlotDto> WorkoutSlots { get; set; } = new();
    }
}
