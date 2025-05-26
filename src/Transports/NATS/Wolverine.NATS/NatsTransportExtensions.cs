using JasperFx.Core.Reflection;
using NATS.Client.Core;
using NATS.Client.Serializers.Json;
using Wolverine.Configuration;

namespace Wolverine.NATS;

public static class NatsTransportExtensions
{
    internal static NatsTransport NatsTransport(this WolverineOptions endpoints)
    {
        var transports = endpoints.As<WolverineOptions>().Transports;

        return transports.GetOrCreate<NatsTransport>();
    }

    public static void UseNats(this WolverineOptions options)
    {
        options.UseNats(_ => { });
    }

    public static void UseNats(this WolverineOptions options, Action<NatsOpts> configure)
    {
        var transport = options.NatsTransport();
        var natsOpts = new NatsOpts
        {
            SerializerRegistry = new NatsJsonSerializerRegistry()
        };
        configure(natsOpts);
        transport.Configure(natsOpts);
    }
    
    public static NatsListenerConfiguration ListenToNatsSubject(this WolverineOptions options, string subject)
    {
        var transport = options.NatsTransport();
        var endpoint = transport.Subjects[NatsSubject.ToUri(subject)];
        endpoint.IsListener = true;

        return new NatsListenerConfiguration(endpoint);
    }
    
    public static NatsSubscriberConfiguration ToNatsSubject(this IPublishToExpression publishing, string subject)
    {
        var transports = publishing.As<PublishingExpression>().Parent.Transports;
        var transport = transports.GetOrCreate<NatsTransport>();
        var endpoint = transport.Subjects[NatsSubject.ToUri(subject)];
        endpoint.IsListener = false;
        publishing.To(endpoint.Uri);

        return new NatsSubscriberConfiguration(endpoint);
    }
}