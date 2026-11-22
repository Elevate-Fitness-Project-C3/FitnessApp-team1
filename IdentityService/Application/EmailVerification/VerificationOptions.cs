using System.ComponentModel.DataAnnotations;

namespace IdentityService.Application.EmailVerification;

public sealed class VerificationOptions
{
    public const string SectionName = "Verification";

    /// <summary>Secret. Comes from env var / user-secrets, never from a committed file.</summary>
    [Required, MinLength(32)]
    public string HmacKey { get; set; } = "";

    [Range(4, 9)] public int CodeLength { get; set; } = 6;
    [Range(1, 1440)] public int ExpiryMinutes { get; set; } = 10;
    [Range(1, 20)] public int MaxAttempts { get; set; } = 5;
    [Range(0, 3600)] public int ResendCooldownSeconds { get; set; } = 60;
}
