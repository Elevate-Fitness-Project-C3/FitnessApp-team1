using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Clients
{
    public interface IFitnessProfileClient
    {
        Task<UserProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default);
    }
}
