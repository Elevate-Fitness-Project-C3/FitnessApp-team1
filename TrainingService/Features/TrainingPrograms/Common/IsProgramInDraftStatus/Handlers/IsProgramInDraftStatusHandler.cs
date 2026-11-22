using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.IsProgramInDraftStatus.Queries;

namespace TrainingService.Features.TrainingPrograms.Common.IsProgramInDraftStatus.Handlers
{
    public class IsProgramInDraftStatusHandler : IRequestHandler<IsProgramInDraftStatusQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;

        public IsProgramInDraftStatusHandler(IGenericRepository<TrainingProgram> programRepo) => _programRepo = programRepo;

        public async Task<RequestResponse<bool>> Handle(IsProgramInDraftStatusQuery request, CancellationToken cancellationToken)
        {
            var programStatus = await _programRepo.GetAll()
                .Where(p => p.Id == request.ProgramId)
                .Select(p => (ProgramStatus?)p.Status)
                .FirstOrDefaultAsync(cancellationToken);

            if (programStatus == null)
                return RequestResponse<bool>.Fail("Program not found.", 404);

            if (programStatus.Value != ProgramStatus.Draft)
                return RequestResponse<bool>.Fail(
                    $"Cannot publish a program with status '{programStatus.Value}'. Only Draft programs can be published.", 409);

            return RequestResponse<bool>.Ok(true, "Program is in Draft status.");
        }
    }
}
