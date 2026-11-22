using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;

namespace TrainingService.DTOs
{
    public class TrainingProgramDto
    {
        public int DurationWeeks { get; set; }
        public int DaysPerWeek { get; set; }
    }
}
