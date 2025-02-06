using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileCollectionRepository
        : IRepository<FileCollectionEntity, Guid, FileCollection>
    {
        Task<DbResult<bool>> IsCollectionFaissSynced(Guid? collectionId = null);
        Task<DbGetManyResult<FileCollection>> GetTopLevelCollectionsForUser(
            Guid userId,
            params string[] relationShips
        );
        Task<DbGetManyResult<FileCollection>> GetManyCollectionsForUserIncludingSelf(
            Guid parentId,
            Guid userId,
            params string[] relationShips
        );
        Task<DbDeleteResult<Guid>> Delete(Guid collectionId, Guid userId);
    }
}
