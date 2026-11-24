using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Options;
using NutritionService.Infrastructure.Clients;
using Xunit;

namespace NutritionService.Tests;

public class FitnessProfileClientTests
{
    private class TestHttpMessageHandler : HttpMessageHandler
    {
        public Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> HandlerFunc { get; set; } =
            (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));

        public HttpRequestMessage? LastRequest { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            return HandlerFunc(request, cancellationToken);
        }
    }

    private static (FitnessProfileClient client, TestHttpMessageHandler handler) CreateClient(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc,
        IHttpContextAccessor? httpContextAccessor = null,
        TestCurrentUserService? currentUserService = null)
    {
        var handler = new TestHttpMessageHandler { HandlerFunc = handlerFunc };
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://fitnessprofileservice:8080")
        };

        var options = Options.Create(new FitnessProfileOptions
        {
            BaseUrl = "http://fitnessprofileservice:8080",
            TdeeEndpoint = "/api/profile/me/tdee"
        });

        var accessor = httpContextAccessor ?? new HttpContextAccessor();
        var userSvc = currentUserService ?? new TestCurrentUserService();
        var logger = TestLoggerFactory.CreateLogger<FitnessProfileClient>();

        var client = new FitnessProfileClient(httpClient, accessor, userSvc, options, logger);
        return (client, handler);
    }

    [Fact]
    public async Task GetTdeeAsync_ForwardsAuthorizationAndUserIdHeaders()
    {
        // Arrange
        var context = new DefaultHttpContext();
        context.Request.Headers["Authorization"] = "Bearer test-jwt-token";
        context.Request.Headers["X-User-Id"] = "22222222-2222-2222-2222-222222222222";
        var accessor = new HttpContextAccessor { HttpContext = context };

        var (client, handler) = CreateClient(
            (_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"tdee\": 2350, \"isProfileComplete\": true}", Encoding.UTF8, "application/json")
            }),
            httpContextAccessor: accessor);

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2350, result.Tdee);
        Assert.NotNull(handler.LastRequest);
        Assert.True(handler.LastRequest.Headers.Contains("Authorization"));
        Assert.Equal("Bearer test-jwt-token", handler.LastRequest.Headers.GetValues("Authorization").First());
        Assert.True(handler.LastRequest.Headers.Contains("X-User-Id"));
        Assert.Equal("22222222-2222-2222-2222-222222222222", handler.LastRequest.Headers.GetValues("X-User-Id").First());
    }

    [Fact]
    public async Task GetTdeeAsync_When404NotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.IsProfileComplete);
        Assert.Null(result.Tdee);
        Assert.Contains("not found", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_When409Conflict_ReturnsIncompleteResult()
    {
        // Arrange (OnboardingGateFilter in FitnessProfileService returns 409 Conflict when incomplete)
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.Conflict)));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.IsProfileComplete);
        Assert.Null(result.Tdee);
        Assert.Contains("incomplete", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_When500InternalServerError_ReturnsUnavailableResult()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Tdee);
        Assert.Contains("error", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_WhenTimeoutOccurs_ReturnsUnavailableResult()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            throw new OperationCanceledException());

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Tdee);
        Assert.Contains("timed out", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_WhenProfileIncomplete_ReturnsIncompleteResult()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"tdee\": 0, \"isProfileComplete\": false}", Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.False(result.IsProfileComplete);
        Assert.Null(result.Tdee);
        Assert.Contains("complete", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_WhenProfileOutdated_ReturnsOutdatedResult()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"tdee\": 2100, \"isOutdated\": true}", Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.IsOutdated);
        Assert.Null(result.Tdee);
        Assert.Contains("update", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetTdeeAsync_WhenWrappedPayload_ParsesSuccessfully()
    {
        // Arrange
        var (client, _) = CreateClient((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\": {\"tdee\": 2450.5, \"isProfileComplete\": true}}", Encoding.UTF8, "application/json")
            }));

        // Act
        var result = await client.GetTdeeAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(2450.5, result.Tdee);
    }
}
