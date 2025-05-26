using NATS.Client.Core;

namespace Wolverine.NATS;

public class NatsEnvelope : Envelope
{
    public NatsEnvelope(NatsMsg<byte[]> message)
    {
        Data = message.Data;
        if (message.Headers is not null)
        {
            foreach (var messageHeader in message.Headers)
            {
                Headers[messageHeader.Key] = messageHeader.Value;
            }
            
            MessageType = Headers[EnvelopeConstants.MessageTypeKey];
        }
    }
}