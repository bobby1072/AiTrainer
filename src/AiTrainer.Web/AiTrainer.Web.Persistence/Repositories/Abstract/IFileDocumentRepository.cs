using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileDocumentRepository : IRepository<FileDocumentEntity, Guid, FileDocument>
    {
        Task<DbGetManyResult<FileDocument>> GetDocumentsBySync(bool syncSate, Guid userId, Guid? collectionId = null);
        Task<DbSaveResult<FileDocument>> CreateOneWithMetaData(
            FileDocument document,
            FileDocumentMetaData metaData
        );
        Task<DbGetManyResult<FileDocumentPartial>> GetTopLevelDocumentPartialsForUser(
            Guid userId,
            params string[] relationShips
        );
        Task<DbGetManyResult<FileDocumentPartial>> GetManyDocumentPartialsByCollectionId(
            Guid collectionId,
            Guid userId,
            params string[] relationShips
        );

        Task<DbGetOneResult<FileDocument>> GetOne(Guid documentId, Guid userId);

    }
}
