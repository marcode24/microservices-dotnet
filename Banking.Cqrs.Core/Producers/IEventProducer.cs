using Banking.Cqrs.Core.Events;

namespace Banking.Cqrs.Core.Producers
{
    public interface IEventProducer
    {
        void Produce(string topic, BaseEvent @event);
    }
}
