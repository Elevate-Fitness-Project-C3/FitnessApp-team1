using MediatR;
using TrainingService.DTOs;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Queries
{
    public record BrowseExercisesQuery(
        string? Name,
        string? TargetMuscle,
        string? TrackingType,
        int PageIndex = 1,
        int PageSize = 10
        ) : IRequest<RequestResponse<PaginatedResult<ExerciseDto>>>
    {
        private const int MaxPageSize = 100;
        public int PageNumber { get; } = PageIndex < 1 ? 1 : PageIndex;
        public int PageSize { get; } = PageSize < 1 || PageSize > MaxPageSize ? 10 : PageSize;
    }
}
