using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.Queries;

namespace TrainingService.Features.TrainingProgramFeature.Handlers
{
    public class GetTrainingProgramByIdQueryHandler:IRequestHandler<GetTrainingProgramByIdQuery, RequestResponse<TrainingProgramDto>>
    {
        private readonly IGenericRepository<TrainingProgram> _trainingProgramRepo;
        public GetTrainingProgramByIdQueryHandler(IGenericRepository<TrainingProgram> trainingProgramRepo)
         =>   _trainingProgramRepo = trainingProgramRepo;
        
        public async Task<RequestResponse<TrainingProgramDto>> Handle(GetTrainingProgramByIdQuery request, CancellationToken cancellationToken)
        {
            var dto = await _trainingProgramRepo.GetAll()
                .Where(p => p.Id == request.ProgramId)
                .Select(p => new TrainingProgramDto
                {
                    DurationWeeks = p.DurationWeeks,
                    DaysPerWeek = p.DaysPerWeek
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                return RequestResponse<TrainingProgramDto>.Fail("Training program not found");  

            return RequestResponse<TrainingProgramDto>.Ok(dto);
        }
    }
}
