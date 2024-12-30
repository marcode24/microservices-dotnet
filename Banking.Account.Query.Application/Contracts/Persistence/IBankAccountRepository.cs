using Banking.Account.Query.Domain;

namespace Banking.Account.Query.Application.Contracts.Persistence
{
    public interface IBankAccountRepository : IAsyncRepository<BankAccount>
    {
        Task<BankAccount> FindByAccountIdentifier(string identifier);
        Task<IEnumerable<BankAccount>> FindByAccountHolder(string accountHolder);
        Task<IEnumerable<BankAccount>> FindByBalanceGreaterThan(double balance);
        Task<IEnumerable<BankAccount>> FindByBalanceLessThan(double balance);
        Task DeleteByIdentifier(string identifier);
        Task DepositBankAccountByIdentifier(BankAccount bankAccount);
        Task WithdrawnBankAccountByIdentifier(BankAccount bankAccount);
    }
}
