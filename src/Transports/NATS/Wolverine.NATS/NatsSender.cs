using NATS.Client.Core;
using Wolverine.Runtime;
using Wolverine.Transports.Sending;

namespace Wolverine.NATS;

public class NatsSender : ISender, IAsyncDisposable
{
    private readonly IWolverineRuntime _runtime;
    private readonly NatsSubject _endpoint;
    private readonly NatsTransport _transport;
    private readonly CancellationToken _cancellation;
    private readonly NatsEnvelopeMapper _mapper;

    public NatsSender(
        IWolverineRuntime runtime, NatsSubject endpoint, NatsTransport transport, CancellationToken cancellation)
    {
        _runtime = runtime;
        _endpoint = endpoint;
        _transport = transport;
        _cancellation = cancellation;
        _mapper = endpoint.BuildMapper(runtime);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public bool SupportsNativeScheduledSend => false;
    public Uri Destination => _endpoint.Uri;

    public async Task<bool> PingAsync()
    {
        try
        {
            if (!_transport.IsConnected)
            {
                return false;
            }

            await _transport.Connection.PingAsync(_cancellation);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public ValueTask SendAsync(Envelope envelope)
    {
        var headers = new NatsHeaders();
        _mapper.MapEnvelopeToOutgoing(envelope, headers);

        return _transport.Connection.PublishAsync(
            _endpoint.Subject, envelope.Data, headers, cancellationToken: _cancellation);
    }
}