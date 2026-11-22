using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Orchestrator;

namespace TrainingService.Features.Workouts.DeleteWorkout.Handlers;

public class DeleteWorkoutHandler : IRequestHandler<DeleteWorkoutOrchestrator, RequestResponse<Unit>>
{
    private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteWorkoutHandler(
        IGenericRepository<WorkoutTemplate> workoutRepo,
        IUnitOfWork unitOfWork, 
        IMediator mediator)
    {
        _workoutRepo = workoutRepo;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }


    public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutOrchestrator request, CancellationToken cancellationToken)
    {
        var checkexists = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);

        if (!checkexists.Success)
            return RequestResponse<Unit>.Fail(checkexists.Message, checkexists.StatusCode);

        var isUsedInPublished = await _mediator.Send(new IsWorkoutUsedInPublishedProgramQuery(request.WorkoutId), cancellationToken);

        if (!isUsedInPublished.Success)
            return RequestResponse<Unit>.Fail(isUsedInPublished.Message, isUsedInPublished.StatusCode);

        if (isUsedInPublished.Data)
            return RequestResponse<Unit>.Fail("Cannot delete this workout because it is used in a published program.", 400);

        var workoutToDelete = new WorkoutTemplate
        {
            Id = request.WorkoutId,
            IsDeleted = true,
            DeletedAt = DateTime.UtcNow
        };

        _workoutRepo.SaveInclude(
            workoutToDelete,
            nameof(WorkoutTemplate.IsDeleted),
            nameof(WorkoutTemplate.DeletedAt)
        );

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return RequestResponse<Unit>.Ok(Unit.Value, "Workout deleted successfully.");
    }
}