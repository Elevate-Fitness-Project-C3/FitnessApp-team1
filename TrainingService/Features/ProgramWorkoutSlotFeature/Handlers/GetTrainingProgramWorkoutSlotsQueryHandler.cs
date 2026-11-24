using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs.BrowseTrainingProgramDetails;
using TrainingService.Features.ProgramWorkoutSlotFeature.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.GetAllWorkouts.Dtos;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Handlers
{
    public class GetTrainingProgramWorkoutSlotsQueryHandler : IRequestHandler<GetTrainingProgramWorkoutSlotsQuery, RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>>
    {
        private readonly IGenericRepository<ProgramWorkoutSlot> _slotRepo;
        public GetTrainingProgramWorkoutSlotsQueryHandler(IGenericRepository<ProgramWorkoutSlot> slotRepo)
        =>  _slotRepo = slotRepo;
        public async Task<RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>> Handle(
    GetTrainingProgramWorkoutSlotsQuery request,
    CancellationToken ct)
        {
            var query = _slotRepo.GetAll()
                .Where(slot =>
                    slot.ProgramId == request.ProgramId &&
                    slot.TrainingProgram.Status == ProgramStatus.Published);

            var pageIndex = Math.Max(1, request.PageNumber);
            var pageSize = request.PageSize < 1 || request.PageSize > 50
                ? 10
                : request.PageSize;

            var totalCount = await query.CountAsync(ct);

            var slots = await query
                .OrderBy(slot => slot.WeekNumber)
                .ThenBy(slot => slot.DayNumber)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(slot => new ProgramWorkoutSlotDto
                {
                    WeekNumber = slot.WeekNumber,
                    DayNumber = slot.DayNumber,

                    Workout = new WorkoutDetialsDto
                    {
                        Id = slot.WorkoutTemplate.Id,
                        Title = slot.WorkoutTemplate.Title,
                        Description = slot.WorkoutTemplate.Description,
                        Difficulty = slot.WorkoutTemplate.Difficulty,
                        EstimatedDurationMinutes =
                            slot.WorkoutTemplate.EstimatedDurationMinutes
                    }
                })
                .ToListAsync(ct);

            var paginatedResult = new PaginatedResult<ProgramWorkoutSlotDto>(
                slots,
                totalCount,
                pageIndex,
                pageSize
            );

            return RequestResponse<PaginatedResult<ProgramWorkoutSlotDto>>
                .Ok(paginatedResult);
        }
    }
}