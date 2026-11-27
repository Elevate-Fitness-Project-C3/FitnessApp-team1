using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Infrastructure.Clients.FitnessProfile
{
    public interface IFitnessProfileClient
    {
        Task<UserProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default);
    }
}
