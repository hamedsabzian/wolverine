using NATS.Client.Core;
using Wolverine.Configuration;
using Wolverine.Transports;

namespace Wolverine.NATS;

public class NatsEnvelopeMapper : EnvelopeMapper<INatsMsg<byte[]>, NatsHeaders>
{
    public NatsEnvelopeMapper(Endpoint endpoint) : base(endpoint)
    {
    }

    protected override void writeOutgoingHeader(NatsHeaders outgoing, string key, string value)
    {
        outgoing[key] = value;
    }

    protected override bool tryReadIncomingHeader(INatsMsg<byte[]> incoming, string key, out string? value)
    {
        if (incoming.Headers?.TryGetLastValue(key, out value) ?? false)
        {
            return true;
        }

        value = null;
        return false;
    }

    protected override void writeIncomingHeaders(INatsMsg<byte[]> incoming, Envelope envelope)
    {
        if (incoming.Headers is not null)
        {
            foreach (var header in incoming.Headers)
            {
                envelope.Headers[header.Key] = header.Value;
            }
        }
    }
}