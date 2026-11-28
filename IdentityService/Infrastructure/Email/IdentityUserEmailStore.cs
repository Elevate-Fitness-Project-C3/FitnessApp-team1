using IdentityService.Application.EmailVerification;
using IdentityService.Domain;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Infrastructure.Email;

public sealed class IdentityUserEmailStore(UserManager<AppUser> userManager) : IUserEmailStore
{
    public async Task<UserEmailInfo?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = userManager.Users.FirstOrDefault(u => u.Email != null && u.Email.ToLower() == normalized);
        if (user is null) return null;
        if (!Guid.TryParse(user.Id, out var userId)) return null;
        return new UserEmailInfo(userId, user.Email ?? normalized, user.EmailConfirmed);
    }

    public async Task MarkEmailConfirmedAsync(Guid userId, CancellationToken ct = default)
    {
        var id = userId.ToString();
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return;
        if (user.EmailConfirmed) return;
        user.EmailConfirmed = true;
        await userManager.UpdateAsync(user);
    }
}
