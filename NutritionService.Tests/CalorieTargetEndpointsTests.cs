using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using NutritionService.Application.Common.Models;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class CalorieTargetEndpointsTests : IClassFixture<NutritionWebApplicationFactory>
{
    private readonly NutritionWebApplicationFactory _factory;

    public CalorieTargetEndpointsTests(NutritionWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetDailyTarget_WithoutAuthorization_Returns401Unauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/nutrition/targets/daily");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetDailyTarget_WithAuthAndGoalPreference_Returns200WithTarget()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _factory.ResetDatabase();
        _factory.Seed(NutritionPreference.Create(userId, NutritionGoal.GradualLoss, DateTime.UtcNow));
        _factory.FitnessProfileClientStub.Result = FitnessProfileResult.Success(2400);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Create(userId));

        // Act
        var response = await client.GetAsync("/api/nutrition/targets/daily");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<DailyCalorieTargetResponse>();
        Assert.NotNull(body);
        Assert.True(body.IsAvailable);
        Assert.Equal(1900, body.TargetCalories); // 2400 - 500
        Assert.Equal(2400, body.BaselineTdee);
        Assert.Equal(-500, body.Adjustment);
        Assert.Equal(NutritionGoal.GradualLoss, body.Goal);
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, body.Disclaimer);
    }

    [Fact]
    public async Task GetDailyTarget_WithAuthButNoGoalPreference_Returns200WithUnavailableStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _factory.ResetDatabase();
        // Do NOT seed preference for this user
        _factory.FitnessProfileClientStub.Result = FitnessProfileResult.Success(2400);

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Create(userId));

        // Act
        var response = await client.GetAsync("/api/nutrition/targets/daily");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<DailyCalorieTargetResponse>();
        Assert.NotNull(body);
        Assert.False(body.IsAvailable);
        Assert.Null(body.TargetCalories);
        Assert.Contains("nutrition goal", body.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, body.Disclaimer);
    }

    [Fact]
    public async Task GetDailyTarget_WhenProfileServiceUnavailable_Returns200WithUnavailableStatus()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _factory.ResetDatabase();
        _factory.Seed(NutritionPreference.Create(userId, NutritionGoal.Maintain, DateTime.UtcNow));
        _factory.FitnessProfileClientStub.Result = FitnessProfileResult.Unavailable();

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestTokens.Create(userId));

        // Act
        var response = await client.GetAsync("/api/nutrition/targets/daily");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<DailyCalorieTargetResponse>();
        Assert.NotNull(body);
        Assert.False(body.IsAvailable);
        Assert.Null(body.TargetCalories);
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, body.Disclaimer);
    }
}
