using Banking.Account.Command.Application.Aggregates;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Handlers;

namespace Banking.Account.Command.Infrastructure.KafkaEvents
{
    public class AccountEventSourcingHandler : IEventSourcingHandler<AccountAggregate>
    {
        private readonly IEventStore _accountEventStore;

        public AccountEventSourcingHandler(IEventStore accountEventStore)
        {
            _accountEventStore = accountEventStore;
        }

        public async Task<AccountAggregate> GetById(string id)
        {
            var aggregate = new AccountAggregate();
            var events = await _accountEventStore.GetEvents(id);

            if (events is not null && events.Any())
            {
                aggregate.ReplayEvents(events);
                var latestVersion = events.Max(e => e.Version);
                aggregate.SetVersion(latestVersion);
            }

            return aggregate;
        }

        public async Task Save(AggregateRoot aggregateRoot)
        {
            await _accountEventStore.SaveEvents(
                aggregateRoot.Id,
                aggregateRoot.GetUncommittedChanges(),
                aggregateRoot.GetVersion()
            );
            aggregateRoot.MarkChangesAsCommitted();
        }
    }
}
