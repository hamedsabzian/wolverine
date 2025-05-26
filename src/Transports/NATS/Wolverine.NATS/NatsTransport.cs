using JasperFx.Core;
using NATS.Client.Core;
using Wolverine.Runtime;
using Wolverine.Transports;

namespace Wolverine.NATS;

public class NatsTransport : TransportBase<NatsSubject>, IAsyncDisposable
{
    private NatsConnection? _connection;
    private const string ProtocolName = "nats";

    public NatsTransport() : this(ProtocolName, "NATS")
    {
    }

    public NatsTransport(string protocol, string name) : base(protocol, name)
    {
        Subjects = new LightweightCache<Uri, NatsSubject>(uri => new NatsSubject(uri, this));
    }

    public NatsOpts Options { get; private set; }
    public LightweightCache<Uri, NatsSubject> Subjects { get; private set; }

    internal NatsConnection Connection => _connection ?? throw new NullReferenceException("Nats connection is null");
    public bool IsConnected => (_connection?.ConnectionState ?? NatsConnectionState.Closed) == NatsConnectionState.Open;

    public ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
            return _connection.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }

    protected override IEnumerable<NatsSubject> endpoints()
    {
        return Subjects;
    }

    protected override NatsSubject findEndpointByUri(Uri uri)
    {
        return Subjects[uri];
    }

    public void Configure(NatsOpts opts)
    {
        Options = opts;
    }

    public override async ValueTask InitializeAsync(IWolverineRuntime runtime)
    {
        _connection = new NatsConnection(Options);
        await _connection.ConnectAsync();

        _connection.ConnectionDisconnected += ConnectionOnConnectionDisconnected;
        _connection.ConnectionOpened += ConnectionOnConnectionOpened;
        _connection.ReconnectFailed += ConnectionOnReconnectFailed;

        await base.InitializeAsync(runtime);
    }

    private ValueTask ConnectionOnReconnectFailed(object? sender, NatsEventArgs args)
    {
        return ValueTask.CompletedTask;
    }

    private ValueTask ConnectionOnConnectionOpened(object? sender, NatsEventArgs args)
    {
        return ValueTask.CompletedTask;
    }

    private ValueTask ConnectionOnConnectionDisconnected(object? sender, NatsEventArgs args)
    {
        return ValueTask.CompletedTask;
    }
}