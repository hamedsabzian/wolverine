using Wolverine.Configuration;

namespace Wolverine.NATS;

public class NatsListenerConfiguration : ListenerConfiguration<NatsListenerConfiguration, NatsSubject>
{
    public NatsListenerConfiguration(NatsSubject subject) : base(subject)
    {
    }

    public NatsListenerConfiguration(Func<NatsSubject> source) : base(source)
    {
    }
}