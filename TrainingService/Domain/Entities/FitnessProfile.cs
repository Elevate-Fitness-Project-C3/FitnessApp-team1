using TrainingService.Domain.Common;
using TrainingService.Domain.Enums;

namespace TrainingService.Domain.Entities
{
    
        public class FitnessProfile:BaseEntity
        {

            public string UserId { get; set; } = string.Empty;

            public ExperienceLevel TrainingExperienceLevel { get; set; }

            public int AvailableDaysPerWeek { get; set; }

            
    }
}
