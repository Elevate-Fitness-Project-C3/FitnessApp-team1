using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.UserTrainingPrograms.GetActiveTrainingProgram.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.UserTrainingPrograms.GetActiveTrainingProgram.Handlers
{
    public class GetActiveTrainingProgramQueryHandler : IRequestHandler<GetActiveTrainingProgramQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<UserTrainingProgram> _userProgramRepo;

        public GetActiveTrainingProgramQueryHandler(
            IGenericRepository<UserTrainingProgram> userProgramRepo)
        {
            _userProgramRepo = userProgramRepo;
        }

        public async Task<RequestResponse<bool>> Handle(GetActiveTrainingProgramQuery request, CancellationToken cancellationToken)
        {
            var hasActive = await _userProgramRepo.GetAll()
            .AnyAsync(up => up.UserId == request.UserId
                         && up.Status == AttemptStatus.Active,
                      cancellationToken);

            return RequestResponse<bool>.Ok(hasActive);
        }
    }
}
