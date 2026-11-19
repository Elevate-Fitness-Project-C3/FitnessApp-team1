using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Common.Workouts.IsWorkoutExists.Queries;
using TrainingService.Features.Common.Workouts.IsWorkoutInPublishedTrainingProgram.Queries;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Commands;

namespace TrainingService.Features.Workouts.DeleteWorkout.Handlers;

public class DeleteWorkoutHandler : IRequestHandler<DeleteWorkoutCommand, RequestResponse<Unit>>
{
    private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;
    private readonly IMediator _mediator;

    public DeleteWorkoutHandler(
        IGenericRepository<WorkoutTemplate> workoutRepo,
        IMediator mediator)
    {
        _workoutRepo = workoutRepo;
        _mediator = mediator;
    }


    public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
    {

        var checkResult = await _mediator.Send(new IsWorkoutExistsQuery(request.WorkoutId), cancellationToken);

        if (!checkResult.Success)
            return RequestResponse<Unit>.Fail(checkResult.Message, checkResult.StatusCode);

        var isUsedResult = await _mediator.Send(new IsWorkoutUsedInPublishedProgramQuery(request.WorkoutId), cancellationToken);
        if (isUsedResult.Success && isUsedResult.Data == true)
        
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

        await _workoutRepo.SaveChangesAsync(cancellationToken);

        return RequestResponse<Unit>.Ok(Unit.Value, "Workout deleted successfully.");
    }
}