using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Dtos;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Handlers
{
    public class GetPublishedProgramsScoringDetailsQueryHandler : IRequestHandler<GetPublishedProgramsDetailsQuery, RequestResponse<List<ProgramScoringDetailsDto>>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;

        public GetPublishedProgramsScoringDetailsQueryHandler(
            IGenericRepository<TrainingProgram> programRepo)
        {
            _programRepo = programRepo;
        }

        public async Task<RequestResponse<List<ProgramScoringDetailsDto>>> Handle(GetPublishedProgramsDetailsQuery request, CancellationToken cancellationToken)
        {
            var programs = await _programRepo.GetAll()
            .Where(p => p.Status == ProgramStatus.Published)
            .Select(p => new ProgramScoringDetailsDto(
                p.Id,
                p.Title,
                p.Goal,
                p.RequiredLevel, 
                p.DaysPerWeek,
                p.Status,
                p.AccessTier))
            .ToListAsync(cancellationToken);

            return RequestResponse<List<ProgramScoringDetailsDto>>.Ok(
                programs,
                "Published programs scoring details retrieved.");
        }
    }
}
