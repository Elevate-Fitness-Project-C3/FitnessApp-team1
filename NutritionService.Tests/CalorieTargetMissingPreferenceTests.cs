using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Common.Options;
using NutritionService.Application.Queries.GetDailyCalorieTarget;
using Xunit;

namespace NutritionService.Tests;

public class CalorieTargetMissingPreferenceTests
{
    private class FakeFitnessProfileClient : IFitnessProfileClient
    {
        public Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(FitnessProfileResult.Success(2500));
    }

    [Fact]
    public async Task Handle_UserHasNoNutritionPreference_ReturnsUnavailableDirectingToSetGoalFirst()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        // NOTE: No preference is seeded for this user in DB!

        var profileClient = new FakeFitnessProfileClient();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            profileClient,
            currentUserService,
            Options.Create(new CalorieTargetOptions()),
            TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>());

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsAvailable);
        Assert.Null(result.TargetCalories);
        Assert.Null(result.BaselineTdee);
        Assert.Null(result.Adjustment);
        Assert.Null(result.Goal);
        Assert.Contains("set a nutrition goal first", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
