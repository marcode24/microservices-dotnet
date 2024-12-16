using Banking.Account.Command.Domain.Common;

namespace Banking.Account.Command.Application.Contracts.Persistence
{
    public interface IMongoRepository<TDocument>
        where TDocument : IDocument
    {
        Task<IEnumerable<TDocument>> GetAll();

        Task<TDocument> GetById(string id);

        Task InserDocument(TDocument document);

        Task UpdateDocument(TDocument document);

        Task DeleteDocument(string id);
    }
}
