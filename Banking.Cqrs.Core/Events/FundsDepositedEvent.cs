namespace Banking.Cqrs.Core.Events
{
    public class FundsDepositedEvent : BaseEvent
    {
        public double Amount { get; set; }
        public FundsDepositedEvent(string id) : base(id)
        {
        }
    }
}
