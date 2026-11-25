using ProgressService.Domain.Entities;
using ProgressService.Features.WeeklyStreakFeature.DTOs;
using ProgressService.Features.WorkoutHistoryFeature.DTOs;
using ProgressService.Features.WorkoutStatisticsFeature.DTOs;
using ProgressService.Shared;
using ProgressService.ViewModels.WeeklyStreak;
using ProgressService.ViewModels.WorkoutHistory;
using ProgressService.ViewModels.WorkoutStatistics;

namespace ProgressService.Mapping;

public static class MappingExtensions
{
    // ═══════════════════════════════════════════════════════
    //  WorkoutHistory MAPPINGS
    // ═══════════════════════════════════════════════════════
    //1.WorkoutHistoryItemDto Mapping
    public static WorkoutHistoryItemDto ToDto(this CompletedWorkoutRecord entity) => new()
    {
        Id = entity.Id,
        WorkoutSessionId = entity.WorkoutSessionId,
        TrainingProgramId = entity.TrainingProgramId,
        ProgramWorkoutSlotId = entity.ProgramWorkoutSlotId,
        CompletedAtUtc = entity.CompletedAtUtc
    };
    //2.WorkoutHistoryItemViewModel Mapping : 
    public static WorkoutHistoryItemViewModel ToViewModel(this WorkoutHistoryItemDto dto) => new()
    {
        Id = dto.Id,
        WorkoutSessionId = dto.WorkoutSessionId,
        TrainingProgramId = dto.TrainingProgramId,
        ProgramWorkoutSlotId = dto.ProgramWorkoutSlotId,
        CompletedAtUtc = dto.CompletedAtUtc
    };

    // ═══════════════════════════════════════════════════════
    //  ProgressSummary MAPPINGS
    // ═══════════════════════════════════════════════════════
    //ProgressSummaryDto Mapping :
    public static ProgressSummaryViewModel ToViewModel(this ProgressSummaryDto dto) => new()
    {
        TotalCompletedWorkouts = dto.TotalCompletedWorkouts,
        LastWorkoutAtUtc = dto.LastWorkoutAtUtc
    };

    // ═══════════════════════════════════════════════════════
    //  Pagination Mapping MAPPINGS
    // ═══════════════════════════════════════════════════════
    public static PaginatedResult<WorkoutHistoryItemViewModel> ToViewModel(
    this PaginatedResult<WorkoutHistoryItemDto> page)
    => PaginatedResult<WorkoutHistoryItemViewModel>.Create(
        page.Items.Select(i => i.ToViewModel()).ToList(),
        page.TotalCount,
        page.PageIndex,
        page.PageSize);


    // ═══════════════════════════════════════════════════════
    //  WeeklyStreak MAPPINGS
    // ═══════════════════════════════════════════════════════
    public static StreakViewModel ToViewModel(this StreakDto dto) => new()
    {
        CurrentStreak = dto.CurrentStreak,
        LongestStreak = dto.LongestStreak
    };


}