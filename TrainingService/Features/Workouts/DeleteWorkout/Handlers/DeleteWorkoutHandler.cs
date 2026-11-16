using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Commands;
using TrainingService.Features.Workouts.DeleteWorkout.Queries;

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