using MediatR;
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
