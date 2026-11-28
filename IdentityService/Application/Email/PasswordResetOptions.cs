namespace IdentityService.Application;

public sealed class PasswordResetOptions
{
    public int LifetimeMinutes { get; set; } = 30;
    public string ResetUrlBase { get; set; } = "http://localhost:3000/reset-password";
}