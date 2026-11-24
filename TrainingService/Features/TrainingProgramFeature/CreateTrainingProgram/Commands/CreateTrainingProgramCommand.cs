using MediatR;
using TrainingService.Domain.Enums;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.CreateTrainingProgram.Commands
{
    public record CreateTrainingProgramCommand(
        string Title,
        string? Description,
        FitnessGoal FitnessGoal,
        ExperienceLevel ExperienceLevel,
        int DurationWeeks,
        int DaysPerWeek,
        AccessTier AccessTier) : IRequest<RequestResponse<Guid>>;

}
