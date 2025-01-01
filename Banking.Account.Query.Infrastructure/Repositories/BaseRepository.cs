using Banking.Account.Query.Application.Contracts.Persistence;
using Banking.Account.Query.Domain;
using Banking.Account.Query.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Banking.Account.Query.Infrastructure.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : BaseDomainModel
    {
        protected readonly MySqlDbContext _dbContext;

        public BaseRepository(MySqlDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public void AddEntity(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public void DeleteEntity(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return (await _dbContext.Set<T>().FindAsync(id))!;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public void UpdateEntity(T entity)
        {
            _dbContext.Set<T>().Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
