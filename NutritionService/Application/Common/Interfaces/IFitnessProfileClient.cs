using NutritionService.Application.Common.Models;

namespace NutritionService.Application.Common.Interfaces;

/// <summary>
/// Client interface for synchronous internal communication with Fitness Profile Service.
/// </summary>
public interface IFitnessProfileClient
{
    /// <summary>
    /// Fetches the fresh TDEE and completeness status of the current user's profile.
    /// </summary>
    Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default);
}
