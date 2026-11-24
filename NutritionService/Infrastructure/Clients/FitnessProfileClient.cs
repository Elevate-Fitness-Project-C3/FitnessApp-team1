using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Common.Options;

namespace NutritionService.Infrastructure.Clients;

/// <summary>
/// Typed HTTP client for interacting with Fitness Profile Service.
/// </summary>
public class FitnessProfileClient : IFitnessProfileClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOptions<FitnessProfileOptions> _options;
    private readonly ILogger<FitnessProfileClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public FitnessProfileClient(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService,
        IOptions<FitnessProfileOptions> options,
        ILogger<FitnessProfileClient> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
        _options = options;
        _logger = logger;
    }

    public async Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default)
    {
        var endpoint = _options.Value.TdeeEndpoint?.TrimStart('/') ?? "api/profile/me/tdee";
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        ForwardSecurityHeaders(request);

        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Fitness Profile returned 404 Not Found for current user.");
                return FitnessProfileResult.NotFound("Fitness profile not found. Please complete your fitness profile first.");
            }

            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                _logger.LogWarning("Fitness Profile returned 409 Conflict (onboarding incomplete) for current user.");
                return FitnessProfileResult.Incomplete("Fitness profile onboarding is incomplete. Please complete your fitness profile first.");
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Fitness Profile returned non-success HTTP status code: {StatusCode}", response.StatusCode);
                return FitnessProfileResult.Unavailable($"Fitness Profile Service is currently unavailable due to an error (Status {(int)response.StatusCode}). Please try again later or check that your fitness profile is up to date.");
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning("Fitness Profile returned empty response body.");
                return FitnessProfileResult.Incomplete();
            }

            var apiResponse = JsonSerializer.Deserialize<FitnessProfileApiResponse>(content, JsonOptions);
            if (apiResponse is null)
            {
                _logger.LogWarning("Unable to deserialize Fitness Profile response payload.");
                return FitnessProfileResult.Incomplete();
            }

            var isOutdated = apiResponse.IsOutdated ?? apiResponse.Data?.IsOutdated ?? false;
            if (isOutdated)
            {
                return FitnessProfileResult.Outdated("Fitness profile is outdated. Please update your fitness profile to receive an accurate calorie target.");
            }

            var isComplete = apiResponse.IsProfileComplete
                             ?? apiResponse.IsComplete
                             ?? apiResponse.Data?.IsProfileComplete
                             ?? apiResponse.Data?.IsComplete
                             ?? true;

            var tdee = apiResponse.Tdee ?? apiResponse.Data?.Tdee;

            if (!isComplete || !tdee.HasValue || tdee.Value <= 0)
            {
                var msg = apiResponse.Message ?? apiResponse.Data?.Message
                          ?? "Fitness profile is incomplete. Please complete your fitness profile to receive a calorie target.";
                return FitnessProfileResult.Incomplete(msg);
            }

            return FitnessProfileResult.Success(tdee.Value);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(ex, "Request to Fitness Profile Service timed out after options limit.");
            return FitnessProfileResult.Unavailable("Request to Fitness Profile Service timed out. Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "HTTP connection error occurred while requesting TDEE from Fitness Profile Service.");
            return FitnessProfileResult.Unavailable("Unable to connect to Fitness Profile Service. Please try again later.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while communicating with Fitness Profile Service.");
            return FitnessProfileResult.Unavailable("An unexpected error occurred while communicating with Fitness Profile Service.");
        }
    }

    private void ForwardSecurityHeaders(HttpRequestMessage request)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext is null)
        {
            // If running without HttpContext but user service has UserId (e.g. background task/test)
            if (_currentUserService.UserId.HasValue)
            {
                request.Headers.TryAddWithoutValidation("X-User-Id", _currentUserService.UserId.Value.ToString());
            }
            return;
        }

        // Forward Authorization Bearer token
        if (httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader) &&
            !string.IsNullOrWhiteSpace(authHeader))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authHeader.ToString());
        }

        // Forward or provide X-User-Id header
        if (httpContext.Request.Headers.TryGetValue("X-User-Id", out var userIdHeader) &&
            !string.IsNullOrWhiteSpace(userIdHeader))
        {
            request.Headers.TryAddWithoutValidation("X-User-Id", userIdHeader.ToString());
        }
        else if (_currentUserService.UserId.HasValue)
        {
            request.Headers.TryAddWithoutValidation("X-User-Id", _currentUserService.UserId.Value.ToString());
        }
    }

    internal class FitnessProfileApiResponse
    {
        [JsonPropertyName("tdee")]
        public double? Tdee { get; set; }

        [JsonPropertyName("isProfileComplete")]
        public bool? IsProfileComplete { get; set; }

        [JsonPropertyName("isComplete")]
        public bool? IsComplete { get; set; }

        [JsonPropertyName("isOutdated")]
        public bool? IsOutdated { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("data")]
        public FitnessProfileApiResponse? Data { get; set; }
    }
}
