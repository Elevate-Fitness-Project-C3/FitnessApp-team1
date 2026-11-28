namespace IdentityService.Application.Emails;

public interface IEmailQueue
{
    void Enqueue(QueuedEmail email);
}

public interface IEmailQueueReader
{
    IAsyncEnumerable<QueuedEmail> ReadAllAsync(CancellationToken ct);
}