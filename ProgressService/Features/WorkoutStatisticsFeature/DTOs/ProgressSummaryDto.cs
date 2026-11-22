namespace ProgressService.Features.WorkoutStatisticsFeature.DTOs
{
    public class ProgressSummaryDto
    {
        public int TotalCompletedWorkouts { get; set; }
        public DateTime? LastWorkoutAtUtc { get; set; }   // null = "Not yet trained"
    }
}
