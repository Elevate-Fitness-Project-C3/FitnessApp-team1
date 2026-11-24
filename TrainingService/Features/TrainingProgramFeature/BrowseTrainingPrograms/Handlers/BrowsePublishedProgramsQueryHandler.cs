using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.BrowseTrainingPrograms.Queries;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingPrograms.Handlers
{
    public class BrowsePublishedProgramsQueryHandler:IRequestHandler<BrowsePublishedProgramsQuery, RequestResponse<PaginatedResult<BrowseTrainingProgramDto>>>
    {
        private readonly IGenericRepository<TrainingProgram> _trainingProgramRepo;
        public BrowsePublishedProgramsQueryHandler(IGenericRepository<TrainingProgram> trainingProgramRepo)
        {
            _trainingProgramRepo = trainingProgramRepo;
        }
        public async Task<RequestResponse<PaginatedResult<BrowseTrainingProgramDto>>> Handle(BrowsePublishedProgramsQuery request, CancellationToken ct)
        {
            var query = _trainingProgramRepo.GetAll()
                .Where(tp => tp.Status == ProgramStatus.Published);

            if (!string.IsNullOrEmpty(request.SearchTerm))
                query = query.Where(tp => tp.Title.Contains(request.SearchTerm) || (tp.Description != null && tp.Description.Contains(request.SearchTerm)));
            
            if (!string.IsNullOrEmpty(request.TargetGoal))
            {
                if (Enum.TryParse<FitnessGoal>(request.TargetGoal,true, out var goal))
                {
                    query = query.Where(tp => tp.Goal == goal);
                }
            }
            if (!string.IsNullOrEmpty(request.ExperienceLevel))
            {
                if (Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, out var level))
                {
                    query = query.Where(tp => tp.RequiredLevel == level);
                }
            }
            var pageIndex = Math.Max(1, request.PageIndex);
            var pageSize = request.PageSize < 1 || request.PageSize > 50 ? 10 : request.PageSize;
            var total = await query.CountAsync(ct);

            var totalCount = await query.CountAsync(ct);
      

            var programDtos = await query
                .OrderBy(pt=>pt.Title)
                .ThenBy(pt=>pt.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(tp => new BrowseTrainingProgramDto
                {
                    Id = tp.Id,
                    Title = tp.Title,
                    Description = tp.Description,
                    Goal = tp.Goal,
                    RequiredLevel = tp.RequiredLevel,
                    DurationWeeks = tp.DurationWeeks,
                    DaysPerWeek = tp.DaysPerWeek,
                    AccessTier = tp.AccessTier
                })
                .ToListAsync(ct);
            var paginatedResult = new PaginatedResult<BrowseTrainingProgramDto>
            (
                programDtos,
                totalCount,
                pageIndex,
                pageSize
            );
            return RequestResponse<PaginatedResult<BrowseTrainingProgramDto>>.Ok(paginatedResult);
        }
    }
}
