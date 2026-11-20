using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Commands
{
    public record CreateProgramWorkoutSlotCommand(
        Guid ProgramId,
        Guid WorkoutId,
        int WeekNumber,
        int DayOfWeek) : IRequest<RequestResponse<Guid>>;
}
