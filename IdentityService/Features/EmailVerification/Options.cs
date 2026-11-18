namespace IdentityService.Features.EmailVerification;

public sealed class VerificationOptions
{
    public const string SectionName = "Verification";

    public string HmacKey { get; set; } = string.Empty;
    public int CodeLength { get; set; } = 6;
    public int ExpiryMinutes { get; set; } = 10;
    public int MaxAttempts { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
}

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    /// <summary>"Smtp" (default) or "Logging" (does not send anything).</summary>
    public string Provider { get; set; } = "Smtp";
    public string From { get; set; } = "no-reply@local.test";
    public SmtpOptions Smtp { get; set; } = new();
}

public sealed class SmtpOptions
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 1025;
    public bool UseStartTls { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
}
