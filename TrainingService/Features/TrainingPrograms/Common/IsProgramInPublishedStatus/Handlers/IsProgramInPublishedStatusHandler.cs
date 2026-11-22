using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.IsProgramInPublishedStatus.Queries;

namespace TrainingService.Features.TrainingPrograms.Common.IsProgramInPublishedStatus.Handlers
{
    public class IsProgramInPublishedStatusHandler : IRequestHandler<IsProgramInPublishedStatusQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;

        public IsProgramInPublishedStatusHandler(IGenericRepository<TrainingProgram> programRepo) => _programRepo = programRepo;

        public async Task<RequestResponse<bool>> Handle(IsProgramInPublishedStatusQuery request, CancellationToken cancellationToken)
        {
            var programStatus = await _programRepo.GetAll()
           .Where(p => p.Id == request.ProgramId)
           .Select(p => (ProgramStatus?)p.Status)
           .FirstOrDefaultAsync(cancellationToken);

            if (programStatus == null)
                return RequestResponse<bool>.Fail("Program not found.", 404);

            if (programStatus.Value != ProgramStatus.Published)
                return RequestResponse<bool>.Fail(
                    $"Cannot archive a program with status '{programStatus.Value}'. Only Published programs can be archived.",409);

            return RequestResponse<bool>.Ok(true, "Program is in Published status.");
        }
    }
}
