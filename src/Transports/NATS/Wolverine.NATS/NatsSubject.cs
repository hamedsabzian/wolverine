using Wolverine.Configuration;
using Wolverine.Runtime;
using Wolverine.Transports;
using Wolverine.Transports.Sending;

namespace Wolverine.NATS;

public class NatsSubject : Endpoint
{
    private readonly NatsTransport _transport;

    public NatsSubject(string subject, NatsTransport transport)
        : base(ToUri(subject), EndpointRole.Application)
    {
        EndpointName = subject;
        _transport = transport;
    }

    public NatsSubject(Uri uri, NatsTransport transport)
        : base(uri, EndpointRole.Application)
    {
        EndpointName = uri.Segments[1].TrimEnd('/');
        _transport = transport;
    }

    public override ValueTask<IListener> BuildListenerAsync(IWolverineRuntime runtime, IReceiver receiver)
    {
        var listener = new NatsListener(
            runtime, this, receiver, _transport, runtime.Cancellation);
        return ValueTask.FromResult<IListener>(listener);
    }

    protected override ISender CreateSender(IWolverineRuntime runtime)
    {
        return new NatsSender(runtime, this, _transport, runtime.Cancellation);
    }

    public string Subject => EndpointName;

    internal NatsEnvelopeMapper BuildMapper(IWolverineRuntime runtime)
    {
        return new NatsEnvelopeMapper(this);
    }
    
    internal static Uri ToUri(string subject) => new($"nats://subject/{subject.TrimEnd('/')}");
}