using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NutritionService.Application.Common.Interfaces;

namespace NutritionService.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHostEnvironment _environment;

    // Fallback Dev User ID for unauthenticated local development testing only
    private static readonly Guid FallbackDevUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, IHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                            ?? user.FindFirst("sub")
                            ?? user.FindFirst("id")
                            ?? user.FindFirst("userId");

                if (claim != null && Guid.TryParse(claim.Value, out var guid))
                {
                    return guid;
                }
            }

            // Fallback Dev User ID strictly in development environment
            if (_environment.IsDevelopment())
            {
                return FallbackDevUserId;
            }

            return null;
        }
    }

    public Guid GetRequiredUserId()
    {
        return UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
