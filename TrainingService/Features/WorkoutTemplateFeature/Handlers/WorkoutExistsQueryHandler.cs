using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.WorkoutTemplateFeature.Queries;

namespace TrainingService.Features.WorkoutTemplateFeature.Handlers
{
    public class WorkoutExistsQueryHandler : IRequestHandler<WorkoutExistsQuery, RequestResponse<bool>>
    {
        private readonly IGenericRepository<WorkoutTemplate> _workoutTemplateRepo;
        public WorkoutExistsQueryHandler(IGenericRepository<WorkoutTemplate> workoutTemplateRepo)
        => _workoutTemplateRepo = workoutTemplateRepo;

        public async Task<RequestResponse<bool>> Handle(WorkoutExistsQuery request, CancellationToken cancellationToken)
        {
            var result = await _workoutTemplateRepo.GetByIdAsync(request.WorkoutId);
            if (result == null)
                return RequestResponse<bool>.Fail("Workout not found", 404);

            return RequestResponse<bool>.Ok(true);
        }
    }
}