using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Queries;
using TrainingService.Features.TrainingProgramFeature.GetTrainingProgramById.Handlers;

namespace TrainingService.Features.TrainingProgramFeature.BrowseTrainingProgramDetials.Handlers
{
    public class GetTrainingProgramDetainlsQueryHandler:IRequestHandler<GetTrainingProgramDetainlsQuery, RequestResponse<TrainingProgramDetailsDto>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        public GetTrainingProgramDetainlsQueryHandler(IGenericRepository<TrainingProgram> programRepo) =>
            _programRepo = programRepo;

        public async Task<RequestResponse<TrainingProgramDetailsDto>>Handle(GetTrainingProgramDetainlsQuery request ,CancellationToken ct)
        {
            var trainingProgram = await _programRepo.GetAll()
                .Where(tp => tp.Id == request.programId&& tp.Status == ProgramStatus.Published)
                .Select(tp => new TrainingProgramDetailsDto
                {
                    Id = tp.Id,
                    Title = tp.Title,
                    Description = tp.Description,
                    Goal = tp.Goal,
                    RequiredLevel = tp.RequiredLevel,
                    DurationWeeks = tp.DurationWeeks,
                    DaysPerWeek = tp.DaysPerWeek,
                    AccessTier = tp.AccessTier,
                    IsLocked= tp.AccessTier== AccessTier.Premium
                    
                }).FirstOrDefaultAsync(ct);

            if (trainingProgram == null)
                return RequestResponse<TrainingProgramDetailsDto>.Fail("Program not found.", 404);

            return RequestResponse<TrainingProgramDetailsDto>.Ok(trainingProgram);
        }
    }
}
