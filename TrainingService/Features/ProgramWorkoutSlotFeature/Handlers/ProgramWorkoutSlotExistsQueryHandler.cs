using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ProgramWorkoutSlotFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Handlers
{
    public class ProgramWorkoutSlotExistsQueryHandler:IRequestHandler<ProgramWorkoutSlotExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<ProgramWorkoutSlot> _programWorkoutSlotRepo;
        public ProgramWorkoutSlotExistsQueryHandler(IGenericRepository<ProgramWorkoutSlot> programWorkoutSlotRepo)
        {
            _programWorkoutSlotRepo = programWorkoutSlotRepo;
        }

        public async Task<RequestResponse<bool>> Handle(ProgramWorkoutSlotExistsQuery request, CancellationToken cancellationToken)
        {
            var exists = await _programWorkoutSlotRepo.GetAll()
                .AnyAsync(pws => pws.ProgramId == request.ProgramId && pws.WeekNumber == request.WeekNumber && pws.DayNumber == request.DayNumber, cancellationToken);
            return RequestResponse<bool>.Ok(exists);
        }
    }
}
