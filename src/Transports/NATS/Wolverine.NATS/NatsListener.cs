using NATS.Client.Core;
using Wolverine.Runtime;
using Wolverine.Transports;

namespace Wolverine.NATS;

public class NatsListener : IListener
{
    private readonly NatsSubject _subject;
    private readonly CancellationToken _cancellation;
    private readonly Task? _receivingLoop;
    private readonly CancellationTokenSource _localCancellation;
    private INatsSub<byte[]>? _subscriber;

    public NatsListener(
        IWolverineRuntime runtime,
        NatsSubject subject,
        IReceiver receiver,
        NatsTransport transport,
        CancellationToken cancellation)
    {
        _subject = subject;
        _cancellation = cancellation;

        _localCancellation = new CancellationTokenSource();


        var combined = CancellationTokenSource.CreateLinkedTokenSource(_cancellation, _localCancellation.Token);
        _receivingLoop = Task.Run(async () =>
        {
            _subscriber = await transport.Connection.SubscribeCoreAsync<byte[]>(
                subject: subject.Subject, cancellationToken: combined.Token);

            await foreach (var message in _subscriber.Msgs.ReadAllAsync())
            {
                var envelope = new NatsEnvelope(message);

                await receiver.ReceivedAsync(this, envelope);
            }
        }, combined.Token);
    }

    public ValueTask DisposeAsync()
    {
        _localCancellation.Cancel();

        return ValueTask.CompletedTask;
    }

    public ValueTask CompleteAsync(Envelope envelope)
    {
        return ValueTask.CompletedTask; 
    }

    public ValueTask DeferAsync(Envelope envelope)
    {
        return ValueTask.CompletedTask;
    }

    public Uri Address => _subject.Uri;

    public async ValueTask StopAsync()
    {
        if (_subscriber == null)
        {
            return;
        }

        await _subscriber.UnsubscribeAsync();
    }
}