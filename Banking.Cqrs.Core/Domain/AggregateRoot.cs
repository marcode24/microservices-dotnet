using Banking.Cqrs.Core.Events;

namespace Banking.Cqrs.Core.Domain
{
    public abstract class AggregateRoot
    {
        public string Id { get; set; } = string.Empty;

        private int version = -1;
        List<BaseEvent> Changes = new();

        public int GetVersion() => version;

        public void SetVersion(int version)
        {
            this.version = version;
        }

        public List<BaseEvent> GetUncommittedChanges()
        {
            return Changes;
        }

        public void MarkChangesAsCommitted()
        {
            Changes.Clear();
        }

        public void ApplyChange(BaseEvent @event, bool isNewEvent)
        {
            try
            {
                var ClaseDeEvento = @event.GetType();
                var method = GetType().GetMethod("Apply", new[] { ClaseDeEvento });
                method?.Invoke(this, new object[] { @event });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error applying event {@event.GetType().Name} to aggregate {Id}", ex);
            }
            finally
            {
                if (isNewEvent)
                {
                    Changes.Add(@event);
                }
            }
        }

        public void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach(var evt in events)
            {
                ApplyChange(evt, false);
            }
        }
    }
}
