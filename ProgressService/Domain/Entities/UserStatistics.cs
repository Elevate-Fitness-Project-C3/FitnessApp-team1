namespace ProgressService.Domain.Entities
{
    public class UserStatistics
    {
        public Guid UserId { get; set; }
        public int TotalCompletedWorkouts { get; set; }
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public DateTime? LastWorkoutAtUtc { get; set; } 
        public DateTime UpdatedAtUtc { get; set; }
    }
}
