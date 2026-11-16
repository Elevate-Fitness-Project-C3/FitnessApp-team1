using MediatR;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.DTOs;
using NutritionService.Domain.Entities;
using NutritionService.Infrastructure.Persistence;

namespace NutritionService.Application.Commands.SetNutritionGoal;

public class SetNutritionGoalCommandHandler : IRequestHandler<SetNutritionGoalCommand, NutritionPreferenceResponse>
{
    private readonly NutritionDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public SetNutritionGoalCommandHandler(NutritionDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<NutritionPreferenceResponse> Handle(SetNutritionGoalCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.GetRequiredUserId();
        var preference = await _context.NutritionPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var utcNow = DateTime.UtcNow;

        if (preference is null)
        {
            preference = NutritionPreference.Create(userId, request.Goal, utcNow);
            await _context.NutritionPreferences.AddAsync(preference, cancellationToken);
        }
        else
        {
            preference.UpdateGoal(request.Goal, utcNow);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new NutritionPreferenceResponse(
            HasPreference: true,
            Goal: preference.Goal,
            UpdatedAtUtc: preference.UpdatedAtUtc,
            Message: "Nutrition goal preference updated successfully."
        );
    }
}
