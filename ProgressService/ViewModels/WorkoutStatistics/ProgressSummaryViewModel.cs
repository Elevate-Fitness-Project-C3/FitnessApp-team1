namespace ProgressService.ViewModels.WorkoutStatistics
{
    public class ProgressSummaryViewModel
    {
        public int TotalCompletedWorkouts { get; set; }
        public DateTime? LastWorkoutAtUtc { get; set; }
    }
}
