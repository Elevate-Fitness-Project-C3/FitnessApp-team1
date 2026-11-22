using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitnessApp.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    bool IsAdmin { get; }
}

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId
    {
        get
        {
            var sub = accessor.HttpContext?.User?.FindFirst("sub")?.Value;

            if (!Guid.TryParse(sub, out var userId))
                throw new InvalidOperationException("The authenticated 'sub' claim is missing or is not a Guid.");

            return userId;
        }
    }

    public bool IsAdmin => accessor.HttpContext?.User?.IsInRole(Roles.Admin) == true;
}
