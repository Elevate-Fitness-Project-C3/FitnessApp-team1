using MediatR;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Commands.DeactivateMeal;

public class DeactivateMealCommandHandler : IRequestHandler<DeactivateMealCommand, bool>
{
    private readonly IGenericRepository<Meal> _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeactivateMealCommandHandler(IGenericRepository<Meal> mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeactivateMealCommand request, CancellationToken cancellationToken)
    {
        var meal = await _mealRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meal is null)
        {
            return false;
        }

        meal.Deactivate(DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
