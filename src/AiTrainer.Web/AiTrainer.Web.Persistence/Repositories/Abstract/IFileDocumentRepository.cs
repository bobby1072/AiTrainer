using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileDocumentRepository : IRepository<FileDocumentEntity, Guid, FileDocument>
    {
        Task<DbDeleteResult<FileDocument>> Delete(
            FileDocument document);
        Task<DbGetManyResult<FileDocument>> GetDocumentsBySync(bool syncSate, Guid userId, Guid? collectionId = null, params string[] relations);
        Task<DbSaveResult<FileDocument>> Create(
            FileDocument document,
            FileDocumentMetaData metaData
        );
        Task<DbGetManyResult<FileDocumentPartial>> GetTopLevelDocumentPartialsForUser(
            Guid userId,
            params string[] relationShips
        );
        Task<DbGetManyResult<FileDocumentPartial>> GetManyDocumentPartialsByCollectionIdAndUserId(
            Guid userId,
            Guid? collectionId,
            params string[] relationShips
        );
        Task<DbGetManyResult<FileDocument>> GetManyDocumentsByCollectionIdAndUserId(
            Guid userId,
            Guid? collectionId,
            params string[] relationShips
        );
        

        Task<DbGetOneResult<FileDocument>> GetOne(Guid documentId, Guid userId);

    }
}
