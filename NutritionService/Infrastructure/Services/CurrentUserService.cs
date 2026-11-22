using System.Security.Claims;
using FitnessApp.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using NutritionService.Application.Common.Interfaces;

namespace NutritionService.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUser _currentUser;
    private readonly IHostEnvironment _environment;

    // Fallback Dev User ID for unauthenticated local development testing only
    private static readonly Guid FallbackDevUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUser currentUser,
        IHostEnvironment environment)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUser = currentUser;
        _environment = environment;
    }

    public Guid? UserId
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // 1. First check authenticated claims / _currentUser
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                try
                {
                    return _currentUser.UserId;
                }
                catch (InvalidOperationException)
                {
                    var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)
                                ?? httpContext.User.FindFirst("sub")
                                ?? httpContext.User.FindFirst("id")
                                ?? httpContext.User.FindFirst("userId");

                    if (claim != null && Guid.TryParse(claim.Value, out var guid))
                    {
                        return guid;
                    }

                    throw;
                }
            }

            // 2. If unauthenticated AND in Development environment, check X-User-Id header, then FallbackDevUserId
            if (_environment.IsDevelopment())
            {
                if (httpContext != null &&
                    httpContext.Request.Headers.TryGetValue("X-User-Id", out var headerVal) &&
                    Guid.TryParse(headerVal, out var headerGuid))
                {
                    return headerGuid;
                }

                return FallbackDevUserId;
            }

            // 3. Outside Development, require authentication (never override authenticated identity)
            return null;
        }
    }

    public Guid GetRequiredUserId()
    {
        return UserId ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }

    public bool IsAdmin
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated == true)
            {
                return _currentUser.IsAdmin;
            }

            if (_environment.IsDevelopment())
            {
                var roleHeader = httpContext?.Request.Headers["X-Role"].FirstOrDefault();
                if (string.Equals(roleHeader, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
