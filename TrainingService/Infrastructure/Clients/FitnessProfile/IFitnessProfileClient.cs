using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Infrastructure.Clients
{
    public interface IFitnessProfileClient
    {
        Task<UserProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default);
    }
}
