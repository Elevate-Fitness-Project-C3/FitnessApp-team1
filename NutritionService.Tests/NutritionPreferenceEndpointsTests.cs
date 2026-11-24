using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class NutritionPreferenceEndpointsTests : IClassFixture<NutritionWebApplicationFactory>
{
    private readonly NutritionWebApplicationFactory _factory;

    public NutritionPreferenceEndpointsTests(NutritionWebApplicationFactory factory)
    {
        _factory = factory;
    }

    private HttpClient CreateAuthenticatedClient(Guid userId)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TestTokens.Create(userId));
        return client;
    }

    [Fact]
    public async Task PreferenceEndpoints_WithoutAuthorization_Return401Unauthorized()
    {
        _factory.ResetDatabase();
        var client = _factory.CreateClient();

        var getResponse = await client.GetAsync("/api/nutrition/preferences/me");
        var putResponse = await client.PutAsJsonAsync(
            "/api/nutrition/preferences/me",
            new SetNutritionGoalRequest(NutritionGoal.Maintain));

        Assert.Equal(HttpStatusCode.Unauthorized, getResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, putResponse.StatusCode);
    }

    [Fact]
    public async Task PutThenGetMyPreference_PersistsAndReturnsGoal()
    {
        _factory.ResetDatabase();
        var userId = Guid.NewGuid();
        var client = CreateAuthenticatedClient(userId);

        var putResponse = await client.PutAsJsonAsync(
            "/api/nutrition/preferences/me",
            new SetNutritionGoalRequest(NutritionGoal.GradualGain));

        Assert.Equal(HttpStatusCode.OK, putResponse.StatusCode);

        var getResponse = await client.GetAsync("/api/nutrition/preferences/me");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var body = await getResponse.Content.ReadFromJsonAsync<NutritionPreferenceResponse>();
        Assert.NotNull(body);
        Assert.True(body.HasPreference);
        Assert.Equal(NutritionGoal.GradualGain, body.Goal);
        Assert.NotNull(body.UpdatedAtUtc);
    }

    [Fact]
    public async Task PutMyPreference_ConcurrentUpdatesForSameUser_AllSucceedAndKeepSingleRow()
    {
        _factory.ResetDatabase();
        var userId = Guid.NewGuid();
        _factory.Seed(NutritionPreference.Create(userId, NutritionGoal.Maintain, DateTime.UtcNow.AddMinutes(-5)));

        var goals = new[]
        {
            NutritionGoal.GradualLoss,
            NutritionGoal.GradualGain,
            NutritionGoal.Maintain,
            NutritionGoal.GradualLoss,
            NutritionGoal.GradualGain
        };

        var tasks = goals.Select(goal =>
        {
            var client = CreateAuthenticatedClient(userId);
            return client.PutAsJsonAsync("/api/nutrition/preferences/me", new SetNutritionGoalRequest(goal));
        }).ToArray();

        var responses = await Task.WhenAll(tasks);

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        using var db = _factory.CreateContext();
        var rows = db.NutritionPreferences.Where(p => p.UserId == userId).ToList();
        Assert.Single(rows);
        Assert.Contains(rows[0].Goal, goals);
    }
}
