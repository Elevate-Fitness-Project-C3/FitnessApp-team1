using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Queries;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Commands;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Rules;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Handlers
{
    public class PublishTrainingProgramCommandHandler : IRequestHandler<PublishTrainingProgramCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public PublishTrainingProgramCommandHandler(IGenericRepository<TrainingProgram> programRepo,
        IMediator mediator,
        IUnitOfWork unitOfWork)
        {
            _programRepo = programRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<Unit>> Handle(PublishTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            var structureResult = await _mediator.Send(new GetProgramStructureQuery(request.ProgramId), cancellationToken);

            if (!structureResult.Success)
                return RequestResponse<Unit>.Fail(structureResult.Message, structureResult.StatusCode);  

            var program = structureResult.Data!;

            if (program.Status != ProgramStatus.Draft)
                return RequestResponse<Unit>.Fail(
                    $"Only Draft programs can be published. Current status: {program.Status}.", 409);

            var problems = ProgramPublishRules.Check(program);
            if (problems.Count > 0)
                return RequestResponse<Unit>.Fail("Program is not ready for publishing.", 400, new Dictionary<string, string[]> { ["Program"] = problems.ToArray() });

            _programRepo.SaveInclude(
                new TrainingProgram { Id = program.ProgramId, Status = ProgramStatus.Published },
                nameof(TrainingProgram.Status));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Program published successfully.");
        }

    }
}
