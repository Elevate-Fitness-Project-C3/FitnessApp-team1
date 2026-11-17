using TrainingService.Domain.Entities;
using TrainingService.DTOs;
using TrainingService.Features.Shared;
using TrainingService.ViewModels.Exercise;

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

        public static ExerciseViewModel ToExerciseViewModel(this ExerciseDto dto) => new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            TargetMuscle = dto.TargetMuscle,
            TrackingType = dto.TrackingType
        };

        public static BrowseExercisesViewModel ToBrowseExercisesViewModel(
            this PaginatedResult<ExerciseDto> result) => new()
            {
                Exercises = result.Items
                .Select(dto => dto.ToExerciseViewModel())
                .ToList(),

                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };

    }
}
