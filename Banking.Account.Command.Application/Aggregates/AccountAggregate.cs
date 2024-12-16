using Banking.Account.Command.Application.Features.BankAccounts.Commands.OpenAccount;
using Banking.Cqrs.Core.Domain;
using Banking.Cqrs.Core.Events;

namespace Banking.Account.Command.Application.Aggregates
{
    public class AccountAggregate : AggregateRoot
    {
        public bool Active { get; set; }
        public double Balance { get; set; }

        public AccountAggregate() { }

        public AccountAggregate(OpenAccountCommand openAccountCommand)
        {
            var accountOpenendEvent = new AccountOpenedEvent(
                id: openAccountCommand.Id,
                accountHolder: openAccountCommand.AccountHolder,
                accountType: openAccountCommand.AccountType,
                createdDate: DateTime.Now,
                openingBalance: openAccountCommand.OpeningBalance
            );

            RaiseEvent(accountOpenendEvent);
        }

        public void Apply(AccountOpenedEvent @event)
        {
            Id = @event.Id;
            Active = true;
            Balance = @event.OpeningBalance;
        }

        public void Apply(FundsDepositedEvent @event)
        {
            Id = @event.Id;
            Balance += @event.Amount;
        }

        public void Apply(FundsWithdrawnEvent @event)
        {
            Id = @event.Id;
            Balance -= @event.Amount;
        }

        public void Apply(AccountClosedEvent @event)
        {
            Id = @event.Id;
            Active = false;
        }

        public void DepositFund(double amount)
        {
            if (!Active) throw new InvalidOperationException("Account is not active");
            if (amount <= 0) throw new InvalidOperationException("Deposit amount must be greater than zero");

            var fundsDepositEvent = new FundsDepositedEvent(Id)
            {
                Id = Id,
                Amount = amount
            };

            RaiseEvent(fundsDepositEvent);
        }

        public void WithdrawFunds(double amount)
        {
            if (!Active) throw new InvalidOperationException("Account is not active");
            if (amount <= 0) throw new InvalidOperationException("Withdrawal amount must be greater than zero");
            if (Balance < amount) throw new InvalidOperationException("Insufficient funds");
            var fundsWithdrawnEvent = new FundsWithdrawnEvent(Id)
            {
                Id = Id,
                Amount = amount
            };
            RaiseEvent(fundsWithdrawnEvent);
        }

        public void CloseAccount()
        {
            if (!Active) throw new InvalidOperationException("Account is not active");

            var accountClosedEvent = new AccountClosedEvent(Id);

            RaiseEvent(accountClosedEvent);
        }
    }
}
