namespace ProgressService.Domain.Entities
{
    public class UserAchievement
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime EarnedAtUtc { get; set; }

        public Achievement Achievement { get; set; } = null!;
    }
}
