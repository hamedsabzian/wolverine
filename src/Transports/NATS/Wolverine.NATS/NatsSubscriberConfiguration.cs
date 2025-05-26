using Wolverine.Configuration;

namespace Wolverine.NATS;

public class NatsSubscriberConfiguration : SubscriberConfiguration<NatsSubscriberConfiguration, NatsSubject>
{
    internal NatsSubscriberConfiguration(NatsSubject subject) : base(subject)
    {
    }
}