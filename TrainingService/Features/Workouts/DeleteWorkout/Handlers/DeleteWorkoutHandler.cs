using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.Workouts.DeleteWorkout.Commands;

namespace TrainingService.Features.Workouts.DeleteWorkout.Handlers;

public class DeleteWorkoutHandler : IRequestHandler<DeleteWorkoutCommand, RequestResponse<Unit>>
{
    private readonly IGenericRepository<WorkoutTemplate> _workoutRepo;

    public DeleteWorkoutHandler(IGenericRepository<WorkoutTemplate> workoutRepo)
    {
        _workoutRepo = workoutRepo;
    }

    public async Task<RequestResponse<Unit>> Handle(DeleteWorkoutCommand request, CancellationToken cancellationToken)
    {
        var workout = await _workoutRepo.GetAll()
            .FirstOrDefaultAsync(w => w.Id == request.WorkoutId, cancellationToken);

        if (workout == null)
            return RequestResponse<Unit>.Fail("Workout not found.", 404);

        await _workoutRepo.DeleteAsync(request.WorkoutId, cancellationToken);

        await _workoutRepo.SaveChangesAsync(cancellationToken);

        return RequestResponse<Unit>.Ok(Unit.Value, "Workout deleted successfully.");
    }
}