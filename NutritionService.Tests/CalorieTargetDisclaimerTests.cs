using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Common.Options;
using NutritionService.Application.DTOs;
using NutritionService.Application.Queries.GetDailyCalorieTarget;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class CalorieTargetDisclaimerTests
{
    private class FakeFitnessProfileClient(FitnessProfileResult result) : IFitnessProfileClient
    {
        public Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }

    [Fact]
    public void DisclaimerConstant_HasExpectedExactString()
    {
        const string expected = "This target is a general estimate, not a personalized medical or clinical nutrition prescription.";
        Assert.Equal(expected, DailyCalorieTargetResponse.GeneralEstimateDisclaimer);
    }

    [Fact]
    public async Task Handle_WhenTargetCalculated_DisclaimerIsAlwaysPresent()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.Maintain);

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            new FakeFitnessProfileClient(FitnessProfileResult.Success(2200)),
            currentUserService,
            Options.Create(new CalorieTargetOptions()),
            TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>());

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, result.Disclaimer);
    }

    [Fact]
    public async Task Handle_WhenMissingPreference_DisclaimerIsAlwaysPresent()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            new FakeFitnessProfileClient(FitnessProfileResult.Success(2200)),
            currentUserService,
            Options.Create(new CalorieTargetOptions()),
            TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>());

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsAvailable);
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, result.Disclaimer);
    }

    [Theory]
    [InlineData("404")]
    [InlineData("500")]
    [InlineData("Timeout")]
    [InlineData("Incomplete")]
    [InlineData("Outdated")]
    public async Task Handle_WhenProfileUnavailableInAnyMode_DisclaimerIsAlwaysPresent(string mode)
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualLoss);

        var profileResult = mode switch
        {
            "404" => FitnessProfileResult.NotFound(),
            "500" => FitnessProfileResult.Unavailable(),
            "Timeout" => FitnessProfileResult.Unavailable("Request to Fitness Profile Service timed out. Please try again later."),
            "Incomplete" => FitnessProfileResult.Incomplete(),
            "Outdated" => FitnessProfileResult.Outdated(),
            _ => FitnessProfileResult.Unavailable()
        };

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            new FakeFitnessProfileClient(profileResult),
            currentUserService,
            Options.Create(new CalorieTargetOptions()),
            TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>());

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.False(result.IsAvailable);
        Assert.Equal(DailyCalorieTargetResponse.GeneralEstimateDisclaimer, result.Disclaimer);
    }
}
