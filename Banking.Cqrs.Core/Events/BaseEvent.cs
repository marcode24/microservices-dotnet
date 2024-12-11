using Banking.Cqrs.Core.Messages;

namespace Banking.Cqrs.Core.Events
{
    public class BaseEvent : Message
    {
        public int Version { get; set; }

        public BaseEvent(string id) : base(id)
        {
        }
    }
}
