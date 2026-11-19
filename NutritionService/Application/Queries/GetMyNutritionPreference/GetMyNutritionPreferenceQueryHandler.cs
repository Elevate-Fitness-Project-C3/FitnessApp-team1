using MediatR;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;

namespace NutritionService.Application.Queries.GetMyNutritionPreference;

public class GetMyNutritionPreferenceQueryHandler : IRequestHandler<GetMyNutritionPreferenceQuery, NutritionPreferenceResponse>
{
    private readonly IGenericRepository<NutritionPreference> _repository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyNutritionPreferenceQueryHandler(
        IGenericRepository<NutritionPreference> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<NutritionPreferenceResponse> Handle(GetMyNutritionPreferenceQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var preference = await _repository
            .FirstOrDefaultAsync(p => p.UserId == userId, asNoTracking: true, cancellationToken);

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
