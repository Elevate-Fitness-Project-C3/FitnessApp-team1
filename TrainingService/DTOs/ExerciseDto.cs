using TrainingService.Domain.Enums;

namespace TrainingService.DTOs
{
    public class ExerciseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MuscleGroup TargetMuscle { get; set; } 
        public TrackingType TrackingType { get; set; }     
    }
}
