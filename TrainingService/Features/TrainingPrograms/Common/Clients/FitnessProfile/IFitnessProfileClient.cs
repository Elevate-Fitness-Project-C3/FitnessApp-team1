using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Features.TrainingPrograms.Common.Clients.FitnessProfile
{
    public interface IFitnessProfileClient
    {
        Task<UserProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default);
    }
}
