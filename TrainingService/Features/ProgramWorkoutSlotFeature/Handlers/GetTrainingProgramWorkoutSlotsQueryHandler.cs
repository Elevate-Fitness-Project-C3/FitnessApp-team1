using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.ProgramWorkoutSlotFeature.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Handlers
{
    public class GetTrainingProgramWorkoutSlotsQueryHandler:IRequestHandler<GetTrainingProgramWorkoutSlotsQuery,RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>
    {
        private readonly IGenericRepository<ProgramWorkoutSlot> _slotRepo;
        public GetTrainingProgramWorkoutSlotsQueryHandler(IGenericRepository<ProgramWorkoutSlot> slotRepo)
        {
            _slotRepo = slotRepo;
        }
        public async Task<RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>Handle(GetTrainingProgramWorkoutSlotsQuery request, CancellationToken ct)
        {
            var query = _slotRepo.GetAll()
                .Where(slot => slot.ProgramId == request.ProgramId)
                .OrderBy(slot => slot.WeekNumber)
                .ThenBy(slot => slot.DayNumber)
                .ToListAsync(ct);
            var result = new List<ProgramWorkoutSlotDto>();
            foreach(var solt in GetTrainingProgramWorkoutSlotsOrchestratorHandler)
            {
                var workout = await _medi
            }
        }
    }
}
