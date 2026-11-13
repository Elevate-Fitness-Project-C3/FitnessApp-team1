namespace TrainingService.Domain.Entities;

using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

public class Workout : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public ExperienceLevel Difficulty { get; set; }
    public int EstimatedDurationMinutes { get; set; }

    public ICollection<PlannedExercise> PlannedExercises { get; set; } = new List<PlannedExercise>();
    public ICollection<ProgramDay> ProgramDays { get; set; } = new List<ProgramDay>();
}
