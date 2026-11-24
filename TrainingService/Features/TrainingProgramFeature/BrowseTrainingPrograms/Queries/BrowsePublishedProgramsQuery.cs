using MediatR;
using TrainingService.DTOs;
using TrainingService.Features.Shared;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingPrograms.Queries
{
    public record BrowsePublishedProgramsQuery(
     string? SearchTerm, string? TargetGoal, string? ExperienceLevel,
     int PageIndex = 1, int PageSize = 10)
     : IRequest<RequestResponse<PaginatedResult<BrowseTrainingProgramDto>>>;
}
