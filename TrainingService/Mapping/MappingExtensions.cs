using System.Reflection;
using TrainingService.Domain.Entities;
using TrainingService.DTOs;
using TrainingService.Features.Shared;
using TrainingService.ViewModels.TrainingProgram;

namespace TrainingService.Mapping
{
    public static class MappingExtensions
    {
        //========================
        // TrainingProgram
        //========================


        public static BrowseTrainingProgramsViewModel ToViewModels(
      this PaginatedResult<BrowseTrainingProgramDto> page)
        {
            return new BrowseTrainingProgramsViewModel
            {
                Items = page.Items.Select(dto => new BrowseTrainingProgramViewModel
                {
                    Id = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    Goal = dto.Goal,
                    RequiredLevel = dto.RequiredLevel,
                    DurationWeeks = dto.DurationWeeks,
                    DaysPerWeek = dto.DaysPerWeek,
                    AccessTier = dto.AccessTier
                }).ToList(),
                TotalCount = page.TotalCount,
                PageIndex = page.PageIndex,
                PageSize = page.PageSize
            };
        }
    }


}
