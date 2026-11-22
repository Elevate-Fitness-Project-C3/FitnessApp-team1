using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Queries.BrowseMeals;

public class BrowseMealsQueryHandler : IRequestHandler<BrowseMealsQuery, PaginatedMealsResponse>
{
    private readonly IGenericRepository<Meal> _mealRepository;

    public BrowseMealsQueryHandler(IGenericRepository<Meal> mealRepository)
    {
        _mealRepository = mealRepository;
    }

    public async Task<PaginatedMealsResponse> Handle(BrowseMealsQuery request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize switch
        {
            < 1 => 20,
            > 100 => 100,
            _ => request.PageSize
        };

        var query = _mealRepository.GetTableNoTracking()
            .Where(m => m.IsActive);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(m => m.Name.Contains(search));
        }

        if (request.Type.HasValue)
        {
            query = query.Where(m => m.Type == request.Type.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(m => m.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
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
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling((double)totalCount / pageSize);

        return new PaginatedMealsResponse(
            Items: items,
            TotalCount: totalCount,
            Page: page,
            PageSize: pageSize,
            TotalPages: totalPages
        );
    }
}
