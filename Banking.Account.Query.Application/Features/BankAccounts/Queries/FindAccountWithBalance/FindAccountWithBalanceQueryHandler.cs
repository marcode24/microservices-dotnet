using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountWithBalance
{
    public class FindAccountWithBalanceQueryHandler : IRequestHandler<FindAccountWithBalanceQuery, IEnumerable<BankAccount>>
    {
        private readonly IBankAccountRepository _bankAccountRepository;

        public FindAccountWithBalanceQueryHandler(IBankAccountRepository bankAccountRepository)
        {
            _bankAccountRepository = bankAccountRepository;
        }

        public async Task<IEnumerable<BankAccount>> Handle(FindAccountWithBalanceQuery request, CancellationToken cancellationToken)
        {
            if (request.EqualityType == "GREATER_THAN")
            {
                return await _bankAccountRepository.FindByBalanceGreaterThan(request.Balance);
            }

            return await _bankAccountRepository.FindByBalanceLessThan(request.Balance);
        }
    }
}
