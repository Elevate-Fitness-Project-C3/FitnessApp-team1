namespace NutritionService.Application.Common.Options;

/// <summary>
/// Configuration options for communicating with Fitness Profile Service.
/// </summary>
public class FitnessProfileOptions
{
    public const string SectionName = "FitnessProfile";

    /// <summary>
    /// Base URL of Fitness Profile Service (e.g. "http://localhost:5002" locally or "http://fitnessprofileservice:8080" in Docker).
    /// </summary>
    public string BaseUrl { get; set; } = "http://localhost:5002";

    /// <summary>
    /// Relative endpoint to read current user's TDEE (default: "/api/profile/me/tdee").
    /// </summary>
    public string TdeeEndpoint { get; set; } = "/api/profile/me/tdee";

    /// <summary>
    /// HTTP request timeout in seconds (default: 10s).
    /// </summary>
    public int TimeoutSeconds { get; set; } = 10;
}
