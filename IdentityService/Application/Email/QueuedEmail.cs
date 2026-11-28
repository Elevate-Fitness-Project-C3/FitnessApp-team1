namespace IdentityService.Application.Emails;

public sealed record QueuedEmail(string To, string Subject, string Body);