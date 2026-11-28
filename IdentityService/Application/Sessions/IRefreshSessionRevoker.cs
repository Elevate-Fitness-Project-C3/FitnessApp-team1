namespace IdentityService.Application.Sessions;

public interface IRefreshSessionRevoker
{
    Task RevokeAllAsync(string userId, DateTime nowUtc, CancellationToken ct);
}