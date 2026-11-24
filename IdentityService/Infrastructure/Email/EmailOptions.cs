namespace IdentityService.Infrastructure.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string From { get; set; } = "";
    public SmtpOptions Smtp { get; set; } = new();
}

public sealed class SmtpOptions
{
    public string Host { get; set; } = "";
    public int Port { get; set; } = 25;
    public string? Username { get; set; } // optional (MailHog needs none)
    public string? Password { get; set; } // secret: env var only
}
