using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountByHolder
{
    public class FindAccountByHolderQuery : IRequest<IEnumerable<BankAccount>>
    {
        public string AccountHolder { get; set; } = string.Empty;
    }
}
