using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Commands;
using TrainingService.Features.TrainingPrograms.Common.IsProgramInPublishedStatus.Queries;

namespace TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Handlers
{
    public class ArchiveTrainingProgramCommandHandler : IRequestHandler<ArchiveTrainingProgramCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public ArchiveTrainingProgramCommandHandler(
            IGenericRepository<TrainingProgram> programRepo,
            IMediator mediator,
            IUnitOfWork unitOfWork)
        {
            _programRepo = programRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestResponse<Unit>> Handle(ArchiveTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            var statusCheck = await _mediator.Send(new IsProgramInPublishedStatusQuery(request.ProgramId), cancellationToken);

            if (!statusCheck.Success)
                return RequestResponse<Unit>.Fail(statusCheck.Message, statusCheck.StatusCode);

            var programToUpdate = new TrainingProgram
            {
                Id = request.ProgramId,
                Status = ProgramStatus.Archived
            };

            _programRepo.SaveInclude(programToUpdate, nameof(TrainingProgram.Status));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Program archived successfully.");
        }
    }
}
