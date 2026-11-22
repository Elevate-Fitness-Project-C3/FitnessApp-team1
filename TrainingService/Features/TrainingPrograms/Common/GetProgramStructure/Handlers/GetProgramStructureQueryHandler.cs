using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Dtos;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Queries;

namespace TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Handlers
{
    public class GetProgramStructureQueryHandler : IRequestHandler<GetProgramStructureQuery, RequestResponse<ProgramStructureDto>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        public GetProgramStructureQueryHandler(IGenericRepository<TrainingProgram> programRepo) => _programRepo = programRepo;
        public async Task<RequestResponse<ProgramStructureDto>> Handle(GetProgramStructureQuery request, CancellationToken cancellationToken)
        {
           var data = await _programRepo.GetAll()
             .Where(p => p.Id == request.ProgramId)
                .Select(p => new ProgramStructureDto(
                    p.Id,
                    p.DurationWeeks,
                    p.DaysPerWeek,
                    p.Title,
                    p.Status,                              
                    p.ProgramWorkoutSlots.Select(s => new ProgramSlotDto(
                        s.WeekNumber,
                        s.DayNumber,
                        s.WorkoutTemplate != null,
                        s.WorkoutTemplate != null ? s.WorkoutTemplate.WorkoutExercises.Count : 0
                    )).ToList()
                ))
             .FirstOrDefaultAsync(cancellationToken);

             if (data == null)
                 return RequestResponse<ProgramStructureDto>.Fail("Program not found.", 404);

              return RequestResponse<ProgramStructureDto>.Ok(data, "Program structure retrieved.");
        }
    
    }
}
