namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class TrainingProgram : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public FitnessGoal Goal { get; set; }
    public ExperienceLevel RequiredLevel { get; set; }
    public int DurationWeeks { get; set; }
    public int DaysPerWeek { get; set; }
    public AccessTier AccessTier { get; set; }
    public ProgramStatus Status { get; set; }

    public ICollection<ProgramWorkoutSlot> ProgramWorkoutSlots { get; set; } = new List<ProgramWorkoutSlot>();
    public ICollection<UserTrainingProgram> UserTrainingPrograms { get; set; } = new List<UserTrainingProgram>();
}
