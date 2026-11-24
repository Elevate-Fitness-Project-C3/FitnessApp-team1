using TrainingService.Domain.Enums;

namespace TrainingService.DTOs.BrowseTrainingProgramDetails
{
    public class WorkoutDetialsDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public ExperienceLevel Difficulty { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }
}
