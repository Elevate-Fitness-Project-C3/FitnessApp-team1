using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Dtos;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Handlers
{
    public class GetProgramScoringDetailsQueryHandler : IRequestHandler<GetProgramScoringDetailsQuery, RequestResponse<ProgramScoringDetailsDto>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;

        public GetProgramScoringDetailsQueryHandler(IGenericRepository<TrainingProgram> programRepo)
        {
            _programRepo = programRepo;
        }

        public async Task<RequestResponse<ProgramScoringDetailsDto>> Handle(GetProgramScoringDetailsQuery request, CancellationToken cancellationToken)
        {
            var programDetails = await _programRepo.GetAll()
                .Where(p => p.Id == request.ProgramId)
                .Select(p => new ProgramScoringDetailsDto(
                    p.Id,
                    p.Title,
                    p.Goal,
                    p.RequiredLevel,
                    p.DaysPerWeek,
                    p.Status,
                    p.AccessTier))
                .FirstOrDefaultAsync(cancellationToken);

            if (programDetails is null)
                return RequestResponse<ProgramScoringDetailsDto>.Fail("Program not found.", 404);

            return RequestResponse<ProgramScoringDetailsDto>.Ok(programDetails, "Program scoring details retrieved.");
        }
    }
}

