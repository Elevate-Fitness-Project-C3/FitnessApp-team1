using ProgressService.Domain.Common;

namespace ProgressService.Domain.Entities
{
    public class Achievement : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RuleCode { get; set; } = string.Empty;

        public ICollection<UserAchievement> UserAchievements { get; set; } = new List<UserAchievement>();
    }
}
