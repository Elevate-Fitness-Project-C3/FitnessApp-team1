using TrainingService.Domain.Entities;
using TrainingService.DTOs;

namespace TrainingService.Mapping
{
    public static class MappingExtensions
    {
        //==========================
        // Exercise Mappings
        //==========================

        public static ExerciseDto ToExerciseDto(this Exercise exercise) => new()
        {
            Id = exercise.Id,
            Name = exercise.Name,
            Description = exercise.Description,
            TargetMuscle = exercise.TargetMuscle,
            TrackingType = exercise.TrackingType
        };
    }
}
