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

    public ICollection<ProgramDay> Days { get; set; } = new List<ProgramDay>();
    public ICollection<ProgramAttempt> Attempts { get; set; } = new List<ProgramAttempt>();
}
