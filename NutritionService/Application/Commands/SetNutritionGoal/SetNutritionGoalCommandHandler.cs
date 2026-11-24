using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Commands.SetNutritionGoal;

public class SetNutritionGoalCommandHandler : IRequestHandler<SetNutritionGoalCommand, bool>
{
    private readonly IGenericRepository<NutritionPreference> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public SetNutritionGoalCommandHandler(
        IGenericRepository<NutritionPreference> repository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<bool> Handle(SetNutritionGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var preference = await _repository
            .FirstOrDefaultAsync(p => p.UserId == userId, asNoTracking: false, cancellationToken);

        var utcNow = DateTime.UtcNow;

        if (preference is null)
        {
            preference = NutritionPreference.Create(userId, request.Goal, utcNow);
            await _repository.AddAsync(preference, cancellationToken);
        }
        else
        {
            preference.UpdateGoal(request.Goal, utcNow);
        }

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            _unitOfWork.ClearTracker();
            var existing = await _repository
                .FirstOrDefaultAsync(p => p.UserId == userId, asNoTracking: false, cancellationToken);

            if (existing is null)
            {
                throw new InvalidOperationException("Failed to persist nutrition preference due to concurrent modification race condition.", ex);
            }

            existing.UpdateGoal(request.Goal, utcNow);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627);
    }
}
