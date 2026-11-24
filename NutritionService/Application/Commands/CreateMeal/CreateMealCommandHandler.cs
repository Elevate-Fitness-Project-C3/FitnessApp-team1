using MediatR;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Commands.CreateMeal;

public class CreateMealCommandHandler : IRequestHandler<CreateMealCommand, Guid>
{
    private readonly IGenericRepository<Meal> _mealRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMealCommandHandler(IGenericRepository<Meal> mealRepository, IUnitOfWork unitOfWork)
    {
        _mealRepository = mealRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMealCommand request, CancellationToken cancellationToken)
    {
        var meal = Meal.Create(request.Name, request.Type, request.Calories, request.ProteinGrams, DateTime.UtcNow);

        await _mealRepository.AddAsync(meal, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return meal.Id;
    }
}
