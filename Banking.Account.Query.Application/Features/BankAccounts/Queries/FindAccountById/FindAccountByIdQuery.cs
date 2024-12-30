using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Account.Query.Domain;
using MediatR;

namespace Banking.Account.Query.Application.Features.BankAccounts.Queries.FindAccountById
{
    public class FindAccountByIdQuery : IRequest<BankAccount>
    {
        public string Identifier { get; set; } = string.Empty;
    }
}
