
namespace TrainingService.DTOs.BrowseTrainingProgramDetails
{
    public class ProgramWorkoutSlotDto
    {
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }

        public WorkoutDetialsDto Workout { get; set; } = null!;
    }
}
