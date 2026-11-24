using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Common.Options;
using NutritionService.Application.Queries.GetDailyCalorieTarget;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class CalorieTargetUnavailableProfileTests
{
    private class FakeFitnessProfileClient : IFitnessProfileClient
    {
        public FitnessProfileResult Result { get; set; } = FitnessProfileResult.Unavailable();

        public Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Result);
    }

    [Fact]
    public async Task Handle_ProfileNotFound_ReturnsUnavailableAndNeverInventsCalories()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.Maintain);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.NotFound("Fitness profile not found. Please complete your fitness profile first.")
        };

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
        Assert.Null(result.TargetCalories); // NEVER fabricates or invents a fallback calorie number!
        Assert.Null(result.BaselineTdee);
        Assert.Null(result.Adjustment);
        Assert.Equal(NutritionGoal.Maintain, result.Goal);
        Assert.Contains("complete", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ProfileService500_ReturnsUnavailableAndNeverInventsCalories()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualLoss);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Unavailable("Fitness Profile Service is currently unavailable due to an error (500). Please try again later.")
        };

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
        Assert.Equal(NutritionGoal.GradualLoss, result.Goal);
        Assert.Contains("unavailable", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_ProfileServiceTimeout_ReturnsUnavailableAndNeverInventsCalories()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualGain);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Unavailable("Request to Fitness Profile Service timed out. Please try again later.")
        };

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
        Assert.Equal(NutritionGoal.GradualGain, result.Goal);
        Assert.Contains("timed out", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_IncompleteProfile_ReturnsUnavailableDirectingToCompleteProfile()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.Maintain);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Incomplete("Fitness profile is incomplete. Please complete your fitness profile to receive a calorie target.")
        };

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
        Assert.Contains("complete", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Handle_OutdatedProfile_ReturnsUnavailableDirectingToUpdateProfile()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualLoss);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Outdated("Fitness profile is outdated. Please update your fitness profile to receive an accurate calorie target.")
        };

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
        Assert.Contains("update", result.Message, StringComparison.OrdinalIgnoreCase);
    }
}
