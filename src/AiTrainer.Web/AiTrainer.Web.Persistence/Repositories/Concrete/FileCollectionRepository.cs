using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionRepository
        : BaseRepository<FileCollectionEntity, Guid, FileCollection>,
            IFileCollectionRepository
    {
        public FileCollectionRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<FileCollectionRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override FileCollectionEntity RuntimeToEntity(FileCollection runtimeObj)
        {
            return runtimeObj.ToEntity();
        }

        public async Task<DbGetManyResult<FileCollection>> GetTopLevelCollectionsForUser(
            Guid userId,
            params string[] relationShips
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var setToQuery = AddRelationsToSet(dbContext.FileCollections, relationShips);

            var entities = await TimeAndLogDbOperation(
                () =>
                    setToQuery.Where(x => x.UserId == userId && x.ParentId == null).ToArrayAsync(),
                nameof(GetTopLevelCollectionsForUser),
                _entityType.Name
            );

            return new DbGetManyResult<FileCollection>(
                entities?.FastArraySelect(x => x.ToModel()).ToArray()
            );
        }

        public async Task<DbGetManyResult<FileCollection>> GetManyCollectionsForUserIncludingSelf(
            Guid parentId,
            Guid userId,
            params string[] relationShips
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var setToQuery = AddRelationsToSet(dbContext.FileCollections, relationShips);

            var entities = await TimeAndLogDbOperation(
                () =>
                    setToQuery
                        .Where(x =>
                            x.UserId == userId && (x.ParentId == parentId || x.Id == parentId)
                        )
                        .ToArrayAsync(),
                nameof(GetTopLevelCollectionsForUser),
                _entityType.Name
            );

            return new DbGetManyResult<FileCollection>(
                entities?.FastArraySelect(x => x.ToModel()).ToArray()
            );
        }

        public async Task<DbDeleteResult<Guid>> Delete(Guid collectionId, Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var deleted = await TimeAndLogDbOperation(
                () =>
                    dbContext
                        .FileCollections.Where(x => x.Id == collectionId && x.UserId == userId)
                        .ExecuteDeleteAsync(),
                nameof(Delete),
                _entityType.Name
            );

            return new DbDeleteResult<Guid>([collectionId]);
        }
    }
}
