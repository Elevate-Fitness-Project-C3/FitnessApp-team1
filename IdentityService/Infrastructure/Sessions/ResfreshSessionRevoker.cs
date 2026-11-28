using IdentityService.Application.Sessions;

namespace IdentityService.Infrastructure.Sessions;

public sealed class RefreshSessionRevoker : IRefreshSessionRevoker
{
    public Task RevokeAllAsync(string userId, DateTime nowUtc, CancellationToken ct)
    {
        // TODO: Wire this to the RefreshToken table from ID-01 when refresh-token persistence is implemented.
        return Task.CompletedTask;
    }
}