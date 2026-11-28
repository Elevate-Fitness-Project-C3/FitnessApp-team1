using System.Threading.Channels;
using IdentityService.Application.Emails;

namespace IdentityService.Infrastructure.Emails;

public sealed class EmailQueue : IEmailQueue, IEmailQueueReader
{
    private readonly Channel<QueuedEmail> _channel = Channel.CreateUnbounded<QueuedEmail>();

    public void Enqueue(QueuedEmail email) => _channel.Writer.TryWrite(email);

    public IAsyncEnumerable<QueuedEmail> ReadAllAsync(CancellationToken ct) =>
        _channel.Reader.ReadAllAsync(ct);
}