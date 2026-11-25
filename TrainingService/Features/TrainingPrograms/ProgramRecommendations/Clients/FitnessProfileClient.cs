using System.Net;
using System.Net.Http.Headers;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Clients
{
    public class FitnessProfileClient : IFitnessProfileClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FitnessProfileClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfileDto?> GetProfileAsync(CancellationToken cancellationToken = default)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("User is not authenticated.");

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));

            var response = await _httpClient.GetAsync("/api/profile/me", cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
                return null; 

            if (response.StatusCode == HttpStatusCode.Conflict)
                return null; 

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<UserProfileDto>(cancellationToken);
        }
    }

}
