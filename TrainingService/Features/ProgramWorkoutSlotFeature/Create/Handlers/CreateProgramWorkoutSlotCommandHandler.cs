using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ProgramWorkoutSlotFeature.Create.Commands;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Create.Handlers
{
    public class CreateProgramWorkoutSlotCommandHandler:IRequestHandler<CreateProgramWorkoutSlotCommand,RequestResponse<Guid>>
    {
        private readonly IGenericRepository<ProgramWorkoutSlot> _programWorkoutSlotRepo;
        public CreateProgramWorkoutSlotCommandHandler(IGenericRepository<ProgramWorkoutSlot> programWorkoutSlotRepo)
        {
            _programWorkoutSlotRepo = programWorkoutSlotRepo;
        }
        public async Task<RequestResponse<Guid>> Handle(CreateProgramWorkoutSlotCommand request, CancellationToken cancellationToken)
        {
            if(request.DayNumber < 1 || request.DayNumber > 7)
                return RequestResponse<Guid>.Fail("Day of week must be between 1 and 7.", 400);
           
            var programWorkoutSlot = new ProgramWorkoutSlot
            {
                ProgramId = request.ProgramId,
                WorkoutId = request.WorkoutId,
                DayNumber = request.DayNumber,
                WeekNumber = request.WeekNumber
            };
            _programWorkoutSlotRepo.Add(programWorkoutSlot);
            await _programWorkoutSlotRepo.SaveChangesAsync(cancellationToken);
            return RequestResponse<Guid>.Created(programWorkoutSlot.Id, "Program workout slot created successfully.");
        }
    
    }
}
