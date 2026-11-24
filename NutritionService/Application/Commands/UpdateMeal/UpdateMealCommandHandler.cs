using MediatR;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Commands.UpdateMeal;

public class UpdateMealCommandHandler : IRequestHandler<UpdateMealCommand, bool>
{
    private readonly IGenericRepository<Meal> _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMealCommandHandler(IGenericRepository<Meal> mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMealCommand request, CancellationToken cancellationToken)
    {
        var meal = await _mealRepository.GetByIdAsync(request.Id, cancellationToken);
        if (meal is null)
        {
            return false;
        }

        meal.UpdateDetails(request.Name, request.Type, request.Calories, request.ProteinGrams, DateTime.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}
