using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Abstract
{
    internal abstract class BaseFileRepository<TEnt, TEntId, TModel> : BaseRepository<TEnt, TEntId, TModel>
        where TEnt : BaseEntity<TEntId, TModel>
        where TModel : class
    {
        protected BaseFileRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<BaseFileRepository<TEnt, TEntId, TModel>> logger
        )
            : base(dbContextFactory, logger) { }


        protected static Task<int> UpdateFileColLastUpdate(IQueryable<FileCollectionEntity> set, IReadOnlyCollection<Guid> userIds, IReadOnlyCollection<Guid> collectionIds)
        {
            return set.Where(x => userIds.Contains(x.UserId) && collectionIds.Contains(x.Id)).ExecuteUpdateAsync(x => x.SetProperty(y => y.DateModified, DateTime.UtcNow));
        }
    }
}
