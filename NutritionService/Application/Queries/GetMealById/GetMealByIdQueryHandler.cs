using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Queries.GetMealById;

public class GetMealByIdQueryHandler : IRequestHandler<GetMealByIdQuery, MealDto?>
{
    private readonly IGenericRepository<Meal> _mealRepository;

    public GetMealByIdQueryHandler(IGenericRepository<Meal> mealRepository)
    {
        _mealRepository = mealRepository;
    }

    public async Task<MealDto?> Handle(GetMealByIdQuery request, CancellationToken cancellationToken)
    {
        return await _mealRepository.GetTableNoTracking()
            .Where(m => m.Id == request.Id && m.IsActive)
            .Select(m => new MealDto(
                m.Id,
                m.Name,
                m.Type,
                m.Calories,
                m.ProteinGrams,
                m.IsActive,
                m.CreatedAtUtc,
                m.UpdatedAtUtc
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
