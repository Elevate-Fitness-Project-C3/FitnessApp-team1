using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;

namespace TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Handlers
{
    public class IsWorkoutUsedInPublishedProgramHandler : IRequestHandler<IsWorkoutUsedInPublishedProgramQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<ProgramWorkoutSlot> _programSlotRepo;

        public IsWorkoutUsedInPublishedProgramHandler(IGenericRepository<ProgramWorkoutSlot> programSlotRepo)
        {
            _programSlotRepo = programSlotRepo;
        }

        
        public async Task<RequestResponse<bool>> Handle(IsWorkoutUsedInPublishedProgramQuery request, CancellationToken cancellationToken)
        {
            var isUsed = await _programSlotRepo.GetAll()
                .AnyAsync(slot =>
                    slot.WorkoutId == request.WorkoutId &&
                    slot.TrainingProgram.Status == ProgramStatus.Published,
                    cancellationToken);

            return RequestResponse<bool>.Ok(isUsed);
        }
    }
}
