using Banking.Account.Query.Domain;
using Microsoft.EntityFrameworkCore;

namespace Banking.Account.Query.Infrastructure.Persistence
{
    public class MySqlDbContext : DbContext
    {
        public MySqlDbContext(DbContextOptions<MySqlDbContext> options) : base(options) { }

        public DbSet<BankAccount>? BankAccounts { get; set; }
    }
}
