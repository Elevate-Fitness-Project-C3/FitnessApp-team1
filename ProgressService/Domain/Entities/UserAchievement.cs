using ProgressService.Domain.Common;

namespace ProgressService.Domain.Entities
{
    public class UserAchievement : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid AchievementId { get; set; }
        public DateTime EarnedAtUtc { get; set; }

        public Achievement Achievement { get; set; } = null!;
    }
}
