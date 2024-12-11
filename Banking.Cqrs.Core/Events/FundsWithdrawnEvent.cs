namespace Banking.Cqrs.Core.Events
{
    public class FundsWithdrawnEvent : BaseEvent
    {
        public double Amount { get; set; }
        public FundsWithdrawnEvent(string id) : base(id)
        {
        }
    }
}
