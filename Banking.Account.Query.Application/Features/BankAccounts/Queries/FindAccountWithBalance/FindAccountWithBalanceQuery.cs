using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountWithBalance
{
    public class FindAccountWithBalanceQuery : IRequest<IEnumerable<BankAccount>>
    {
        public double Balance { get; set; }
        public string EqualityType { get; set; } = string.Empty;
    }
}
