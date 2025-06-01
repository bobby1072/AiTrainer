using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    public interface IFileCollectionRepository
        : IRepository<FileCollectionEntity, Guid, FileCollection>
    {
        Task<DbGetManyResult<FileCollection>> GetCollectionWithChildren(Guid collectionId, params string[] relationships);
        Task<DbSaveResult<FileCollection>> CreateWithSharedMembers(FileCollection entObj, IReadOnlyCollection<SharedFileCollectionMember> sharedMembers);
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
