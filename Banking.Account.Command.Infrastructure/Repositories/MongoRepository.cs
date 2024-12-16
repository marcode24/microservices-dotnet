using Banking.Account.Command.Application.Contracts.Persistence;
using Banking.Account.Command.Application.Models;
using Banking.Account.Command.Domain.Common;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Banking.Account.Command.Infrastructure.Repositories
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument>
        where TDocument : IDocument
    {
        protected readonly IMongoCollection<TDocument> _collection;

        public MongoRepository(IOptions<MongoSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            var db = client.GetDatabase(options.Value.Database);
            _collection = db.GetCollection<TDocument>(typeof(TDocument).Name);
        }

        private protected string GetCollectionName(Type documentType)
        {
            return ((BsonCollectionAttribute)documentType
                .GetCustomAttributes(typeof(BsonCollectionAttribute), true)
                .FirstOrDefault()!).CollectionName;
        }

        public async Task DeleteDocument(string id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<IEnumerable<TDocument>> GetAll()
        {
            return await _collection.Find(p => true).ToListAsync();
        }

        public async Task<TDocument> GetById(string id)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task InserDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public Task UpdateDocument(TDocument document)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
            return _collection.FindOneAndReplaceAsync(filter, document);
        }
    }
}
