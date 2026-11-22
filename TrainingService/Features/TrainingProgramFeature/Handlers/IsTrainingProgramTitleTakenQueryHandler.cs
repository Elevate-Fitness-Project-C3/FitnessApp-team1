using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Queries;

namespace TrainingService.Features.TrainingProgramFeature.Handlers
{
    public class IsTrainingProgramTitleTakenQueryHandler : IRequestHandler<IsTrainingProgramTitleTakenQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<TrainingProgram> _trainingProgramRepo;
        public IsTrainingProgramTitleTakenQueryHandler(IGenericRepository<TrainingProgram> trainingProgramRepo)
        => _trainingProgramRepo = trainingProgramRepo;

        public async Task<RequestResponse<bool>> Handle(IsTrainingProgramTitleTakenQuery request, CancellationToken cancellationToken)
        {
            var isTitleTaken = await _trainingProgramRepo.ExistsAsync(tp => tp.Title == request.Title, cancellationToken);
            return RequestResponse<bool>.Ok(isTitleTaken);
        }
    }
}