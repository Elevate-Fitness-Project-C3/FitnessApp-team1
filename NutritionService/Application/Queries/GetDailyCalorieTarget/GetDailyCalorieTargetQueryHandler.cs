using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Options;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;
using NutritionService.Domain.Enums;

namespace NutritionService.Application.Queries.GetDailyCalorieTarget;

/// <summary>
/// Handles retrieving and calculating daily calorie target for the authenticated user.
/// Coordinates goal policy adjustments and safeguards against unsafe extreme deficits.
/// </summary>
public class GetDailyCalorieTargetQueryHandler : IRequestHandler<GetDailyCalorieTargetQuery, DailyCalorieTargetResponse>
{
    private readonly IGenericRepository<NutritionPreference> _preferenceRepository;
    private readonly IFitnessProfileClient _fitnessProfileClient;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOptions<CalorieTargetOptions> _options;
    private readonly ILogger<GetDailyCalorieTargetQueryHandler> _logger;

    public GetDailyCalorieTargetQueryHandler(
        IGenericRepository<NutritionPreference> preferenceRepository,
        IFitnessProfileClient fitnessProfileClient,
        ICurrentUserService currentUserService,
        IOptions<CalorieTargetOptions> options,
        ILogger<GetDailyCalorieTargetQueryHandler> logger)
    {
        _preferenceRepository = preferenceRepository;
        _fitnessProfileClient = fitnessProfileClient;
        _currentUserService = currentUserService;
        _options = options;
        _logger = logger;
    }

    public async Task<DailyCalorieTargetResponse> Handle(GetDailyCalorieTargetQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();

        // 1. Check local nutrition goal preference
        var preference = await _preferenceRepository.GetTableNoTracking()
            .Where(p => p.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (preference is null)
        {
            _logger.LogInformation("User {UserId} has not set a nutrition goal preference.", userId);
            return DailyCalorieTargetResponse.Unavailable(
                goal: null,
                message: "No nutrition goal preference set. Please set a nutrition goal first (Maintain, GradualLoss, GradualGain)."
            );
        }

        // 2. Query fresh TDEE from Fitness Profile Service via internal SYNC HTTP
        var profileResult = await _fitnessProfileClient.GetTdeeAsync(cancellationToken);

        if (!profileResult.IsSuccess || !profileResult.Tdee.HasValue)
        {
            _logger.LogInformation("Fitness profile TDEE unavailable for user {UserId}: {Message}", userId, profileResult.Message);
            return DailyCalorieTargetResponse.Unavailable(
                goal: preference.Goal,
                message: profileResult.Message ?? "Fitness profile is unavailable or incomplete. Please complete your fitness profile to receive a calorie target."
            );
        }

        // 3. Apply Goal-Based Target Policy with CalorieTargetOptions
        var options = _options.Value;
        int baselineTdee = (int)Math.Round(profileResult.Tdee.Value);

        int adjustment = preference.Goal switch
        {
            NutritionGoal.Maintain => options.MaintainAdjustment,
            NutritionGoal.GradualLoss => options.GetLossAdjustment(),
            NutritionGoal.GradualGain => options.GetGainAdjustment(),
            _ => 0
        };

        int rawTarget = baselineTdee + adjustment;
        // Apply minimum calorie floor for health safety
        int finalTarget = Math.Max(options.MinimumCalorieFloor, rawTarget);

        _logger.LogInformation(
            "User {UserId}: Target calculated. TDEE={Tdee}, Goal={Goal}, Adjustment={Adjustment}, Floor={Floor}, FinalTarget={FinalTarget}",
            userId, baselineTdee, preference.Goal, adjustment, options.MinimumCalorieFloor, finalTarget);

        return DailyCalorieTargetResponse.Available(
            targetCalories: finalTarget,
            baselineTdee: baselineTdee,
            adjustment: adjustment,
            goal: preference.Goal
        );
    }
}
