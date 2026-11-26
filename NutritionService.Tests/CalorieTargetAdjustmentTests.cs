using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Models;
using NutritionService.Application.Common.Options;
using NutritionService.Application.Queries.GetDailyCalorieTarget;
using NutritionService.Domain.Enums;
using Xunit;

namespace NutritionService.Tests;

public class CalorieTargetAdjustmentTests
{
    private class FakeFitnessProfileClient : IFitnessProfileClient
    {
        public FitnessProfileResult Result { get; set; } = FitnessProfileResult.Success(2000);

        public Task<FitnessProfileResult> GetTdeeAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(Result);
    }

    [Theory]
    [InlineData(NutritionGoal.Maintain, 2200, 2200, 0)]
    [InlineData(NutritionGoal.GradualLoss, 2200, 1700, -500)]
    [InlineData(NutritionGoal.GradualGain, 2200, 2500, 300)]
    public async Task Handle_ValidTdee_AppliesCorrectGoalAdjustment(
        NutritionGoal goal,
        double tdee,
        int expectedTarget,
        int expectedAdjustment)
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, goal);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Success(tdee)
        };

        var options = Options.Create(new CalorieTargetOptions());
        var logger = TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            profileClient,
            currentUserService,
            options,
            logger);

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsAvailable);
        Assert.Equal(expectedTarget, result.TargetCalories);
        Assert.Equal((int)tdee, result.BaselineTdee);
        Assert.Equal(expectedAdjustment, result.Adjustment);
        Assert.Equal(goal, result.Goal);
    }

    [Fact]
    public async Task Handle_GradualLoss_WhenCalculatedBelowFloor_ClampsToMinimumFloor()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualLoss);

        // TDEE 1400 with -500 deficit = 900, which is below floor 1200
        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Success(1400)
        };

        var options = Options.Create(new CalorieTargetOptions
        {
            LossAdjustment = -500,
            MinimumCalorieFloor = 1200
        });

        var logger = TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            profileClient,
            currentUserService,
            options,
            logger);

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsAvailable);
        Assert.Equal(1200, result.TargetCalories); // Clamped to health safety floor
        Assert.Equal(1400, result.BaselineTdee);
        Assert.Equal(-200, result.Adjustment);
    }

    [Fact]
    public async Task Handle_Maintain_WhenBaselineTdeeBelowFloor_ClampsToMinimumFloor()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.Maintain);

        // TDEE 1050 with 0 adjustment = 1050, below floor 1200
        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Success(1050)
        };

        var options = Options.Create(new CalorieTargetOptions { MinimumCalorieFloor = 1200 });
        var logger = TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            profileClient,
            currentUserService,
            options,
            logger);

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsAvailable);
        Assert.Equal(1200, result.TargetCalories);
        Assert.Equal(1050, result.BaselineTdee);
        Assert.Equal(150, result.Adjustment);
    }

    [Fact]
    public async Task Handle_CustomOptions_AppliesCustomAdjustmentsAndFloor()
    {
        // Arrange
        using var testDb = new TestDatabaseContext();
        var currentUserService = new TestCurrentUserService();
        testDb.SeedPreference(currentUserService.UserId!.Value, NutritionGoal.GradualLoss);

        var profileClient = new FakeFitnessProfileClient
        {
            Result = FitnessProfileResult.Success(2400)
        };

        var options = Options.Create(new CalorieTargetOptions
        {
            DeficitCalories = 650, // overrides LossAdjustment with -650
            MinimumCalorieFloor = 1500
        });

        var logger = TestLoggerFactory.CreateLogger<GetDailyCalorieTargetQueryHandler>();

        var handler = new GetDailyCalorieTargetQueryHandler(
            testDb.PreferenceRepository,
            profileClient,
            currentUserService,
            options,
            logger);

        // Act
        var result = await handler.Handle(new GetDailyCalorieTargetQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsAvailable);
        Assert.Equal(1750, result.TargetCalories); // 2400 - 650
        Assert.Equal(-650, result.Adjustment);
    }
}
