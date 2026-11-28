using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.UserTrainingPrograms.GetActiveTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.SelectTrainingProgram.Handlers
{
    public class StopActiveTrainingProgramCommandHandler: IRequestHandler<StopActiveTrainingProgramCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<UserTrainingProgram> _userProgramRepo;
        private readonly IMediator _mediator;

        public StopActiveTrainingProgramCommandHandler(
            IGenericRepository<UserTrainingProgram> userProgramRepo,
            IMediator mediator)
        {
            _userProgramRepo = userProgramRepo;
            _mediator = mediator;
        }

        public async Task<RequestResponse<Unit>> Handle(StopActiveTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            var oldActive = _mediator.Send(new GetActiveTrainingProgramQuery(request.UserId), cancellationToken);

            if (oldActive is null)
                return RequestResponse<Unit>.Ok(Unit.Value, "No active program to stop.");

            var acrivateProgram = new UserTrainingProgram
            {
                UserId = request.UserId,
                Status = AttemptStatus.Stopped,
                StartedAt = DateTime.UtcNow
            };

            _userProgramRepo.SaveInclude(acrivateProgram,
                nameof(UserTrainingProgram.Status),
                nameof(UserTrainingProgram.StoppedAt));

            return RequestResponse<Unit>.Ok(Unit.Value, "Active program stopped.");
        }
    }
}
