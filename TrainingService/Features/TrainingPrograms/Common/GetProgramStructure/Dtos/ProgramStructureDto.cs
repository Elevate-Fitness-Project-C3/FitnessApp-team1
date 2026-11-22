using TrainingService.Domain.Enums;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Dtos
{
    public record ProgramStructureDto(
       Guid ProgramId,
       int DurationWeeks,
       int DaysPerWeek,
       string Title,
       ProgramStatus Status,
       List<ProgramSlotDto> Slots
   );

    public record ProgramSlotDto(
        int WeekNumber,
        int DayNumber,
        bool HasWorkout,
        int ExercisesCount
    );
}
