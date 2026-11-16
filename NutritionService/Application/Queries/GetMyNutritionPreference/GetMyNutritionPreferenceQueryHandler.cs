using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.DTOs;
using NutritionService.Infrastructure.Persistence;

namespace NutritionService.Application.Queries.GetMyNutritionPreference;

public class GetMyNutritionPreferenceQueryHandler : IRequestHandler<GetMyNutritionPreferenceQuery, NutritionPreferenceResponse>
{
    private readonly NutritionDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNutritionPreferenceQueryHandler(NutritionDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<NutritionPreferenceResponse> Handle(GetMyNutritionPreferenceQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var preference = await _context.NutritionPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (preference is null)
        {
            return new NutritionPreferenceResponse(
                HasPreference: false,
                Goal: null,
                UpdatedAtUtc: null,
                Message: "Please select a nutrition goal (Maintain, GradualLoss, GradualGain)."
            );
        }

        return new NutritionPreferenceResponse(
            HasPreference: true,
            Goal: preference.Goal,
            UpdatedAtUtc: preference.UpdatedAtUtc,
            Message: null
        );
    }
}
