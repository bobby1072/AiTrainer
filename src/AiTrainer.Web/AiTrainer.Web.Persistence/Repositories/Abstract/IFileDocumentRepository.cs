using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileDocumentRepository : IRepository<FileDocumentEntity, Guid, FileDocument>
    {
        Task<DbGetManyResult<FileDocumentPartial>> GetTopLevelDocumentPartialsForUser(
            Guid userId,
            params string[] relationShips
        );
        Task<DbGetManyResult<FileDocumentPartial>> GetManyDocumentPartialsByCollectionId(
            Guid collectionId,
            Guid userId,
            params string[] relationShips
        );

        Task<DbDeleteResult<Guid>> Delete(Guid documentId, Guid userId);
    }
}
