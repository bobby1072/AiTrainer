using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileDocumentRepository : IRepository<FileDocumentEntity, Guid, FileDocument>
    {
        Task<DbGetManyResult<FileDocumentPartial>> GetManyPartialsByCollectionId(Guid collectionId, params string[] relationShips);
    }
}