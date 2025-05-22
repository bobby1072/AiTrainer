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

        public async Task<DbSaveResult<FileCollection>> CreateWithSharedMembers(FileCollection entObj,
            IReadOnlyCollection<SharedFileCollectionMember> sharedMembers)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                await dbContext.FileCollections.AddAsync(entObj.ToEntity());

                await dbContext.SaveChangesAsync();
                var createdFileCol = dbContext.FileCollections.Local.FirstOrDefault();
                
                if (sharedMembers.Count > 0)
                {
                    await dbContext.SharedFileCollectionMembers.AddRangeAsync(
                        sharedMembers.FastArraySelect(x =>
                        {
                            var ent = x.ToEntity();
                            ent.CollectionId = createdFileCol!.Id;
                            return ent;
                        }));
                }
                
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbSaveResult<FileCollection>
                {
                    Data = dbContext.FileCollections.Local.FastArraySelect(x => x.ToModel()).ToArray(),
                    IsSuccessful = true
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<DbGetOneResult<FileCollection>> GetCollectionByUserIdAndCollectionId(Guid userId, Guid collectionId, params string[] relations)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var setToQuery = AddRelationsToSet(dbContext.FileCollections, relations);
            var dbOp = await TimeAndLogDbOperation(() =>
                setToQuery.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == collectionId),
                nameof(GetCollectionByUserIdAndCollectionId),
                _entityType.Name);

            return new DbGetOneResult<FileCollection>(dbOp?.ToModel());

        }
        public async Task<DbResult<bool>> IsCollectionFaissSynced(Guid? collectionId = null)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var dbOp = await TimeAndLogDbOperation(
                () =>
                    dbContext.FileDocuments.AllAsync(x =>
                        x.CollectionId == collectionId && x.FaissSynced
                    ),
                nameof(IsCollectionFaissSynced),
                _entityType.Name
            );
            return new DbResult<bool>(dbOp, true);
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
                    setToQuery
                        .Include(x => x.SharedFileMembers)
                        .Where(x =>
                            (x.UserId == userId || x.SharedFileMembers!.Any(sfm => sfm.UserId == userId))
                            && x.ParentId == null
                        )
                        .ToArrayAsync(),
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
                            (x.UserId == userId || x.SharedFileMembers!.Any(sfm => sfm.UserId == userId)) && (x.ParentId == parentId || x.Id == parentId)
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

            _ = await TimeAndLogDbOperation(
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
