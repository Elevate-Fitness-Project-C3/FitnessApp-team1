namespace FitnessProfileService.Application;

public interface IOnboardingGate
{
    Task<bool> IsCompletedAsync(Guid userId, CancellationToken cancellationToken = default);
}
