using MediatR;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Queries
{
    public record ProgramWorkoutSlotExistsQuery(
        Guid ProgramId,
        int WeekNumber,
        int DayOfWeek) : IRequest<RequestResponse<bool>>;
}
