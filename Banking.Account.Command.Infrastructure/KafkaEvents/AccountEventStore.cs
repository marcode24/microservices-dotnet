using Banking.Account.Command.Application.Aggregates;
using Banking.Account.Command.Application.Contracts.Persistence;
using Banking.Account.Command.Domain;
using Banking.Cqrs.Core.Events;
using Banking.Cqrs.Core.Handlers;
using Banking.Cqrs.Core.Producers;

namespace Banking.Account.Command.Infrastructure.KafkaEvents
{
    public class AccountEventStore : IEventStore
    {
        private readonly IEventStoreRepository _eventStoreRepository;
        private readonly IEventProducer _eventProducer;

        public AccountEventStore(
            IEventStoreRepository eventStoreRepository,
            IEventProducer eventProducer)
        {
            _eventStoreRepository = eventStoreRepository;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEvents(string aggregateId)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdentifier(aggregateId);
            if (eventStream is null || !eventStream.Any())
            {
                throw new Exception("Aggregate not found");
            }

            return eventStream
                .Select(x => x.EventData)
                .ToList()!;
        }

        public async Task SaveEvents(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var eventStream = await _eventStoreRepository.FindByAggregateIdentifier(aggregateId);
            if (expectedVersion != -1 && eventStream.ElementAt(eventStream.Count() - 1).Version != expectedVersion)
            {
                throw new Exception("Concurrency exception");
            }

            var version = expectedVersion;
            foreach (var evt in events)
            {
                version++;
                evt.Version = version;

                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = nameof(AccountAggregate),
                    Version = version,
                    EventType = evt.GetType().Name,
                };

                await _eventStoreRepository.InserDocument(eventModel);
                _eventProducer.Produce(evt.GetType().Name, evt);
            }
        }
    }
}
