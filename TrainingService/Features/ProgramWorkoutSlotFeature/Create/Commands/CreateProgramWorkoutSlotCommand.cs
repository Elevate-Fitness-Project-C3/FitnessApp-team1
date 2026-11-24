using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Create.Commands
{
    public record CreateProgramWorkoutSlotCommand(
        Guid ProgramId,
        Guid WorkoutId,
        int WeekNumber,
        int DayNumber) : IRequest<RequestResponse<Guid>>;
}
