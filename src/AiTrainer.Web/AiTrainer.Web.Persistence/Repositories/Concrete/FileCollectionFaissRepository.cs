using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionFaissRepository : BaseFileRepository<FileCollectionFaissEntity, long, FileCollectionFaiss>, IFileCollectionFaissRepository
    {
        public FileCollectionFaissRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<FileCollectionFaissRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override FileCollectionFaissEntity RuntimeToEntity(FileCollectionFaiss runtimeObj)
        {
            return runtimeObj.ToEntity();
        }
        public async Task<DbGetOneResult<FileCollectionFaiss>> ByUserAndCollectionId(Guid userId, Guid? collectionId, params string[] relations)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var foundResult = await TimeAndLogDbOperation(() =>
                dbContext.FileCollectionFaiss.FirstOrDefaultAsync(x => x.UserId == userId && x.CollectionId == collectionId),
                nameof(ByUserAndCollectionId),
                _entityType.Name
            );

            return new DbGetOneResult<FileCollectionFaiss>(foundResult?.ToModel());
        }
        public async Task<DbResult> DeleteDocumentAndStoreAndUnsyncDocuments(FileDocument documentToDelete)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var updateModDateDbOp = () => documentToDelete.CollectionId is Guid foundColId ? UpdateFileColLastUpdate(
                    dbContext.FileCollections,
                    [documentToDelete.UserId],
                    [foundColId]
                ): Task.CompletedTask; 
                await Task.WhenAll(
                    updateModDateDbOp.Invoke(),
                    dbContext.FileDocuments
                        .Where(x => x.CollectionId == documentToDelete.CollectionId && x.UserId == documentToDelete.UserId)
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.FaissSynced, false)),
                    dbContext.FileCollectionFaiss
                        .Where(x => x.CollectionId == documentToDelete.CollectionId && x.UserId == documentToDelete.UserId)
                        .ExecuteDeleteAsync(),
                    dbContext.FileDocuments
                        .Where(x => x.Id == documentToDelete.Id)
                        .ExecuteDeleteAsync()
                );

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbResult(true);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<DbResult> SaveStoreAndSyncDocs(FileCollectionFaiss fileCollectionFaiss, IReadOnlyCollection<SingleDocumentChunk> newChunks, IReadOnlyCollection<Guid> documentIdsToSync,
            FileCollectionFaissRepositorySaveMode saveMode)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var singleChunkEntity = newChunks.FastArraySelect(x => x.ToEntity());
                var faissEntity = fileCollectionFaiss.ToEntity();
                Func<Task<EntityEntry<FileCollectionFaissEntity>>> saveFunc = async () => saveMode == FileCollectionFaissRepositorySaveMode.Create ? await dbContext.FileCollectionFaiss
                    .AddAsync(faissEntity) : dbContext.FileCollectionFaiss.Update(faissEntity);
                
                await Task.WhenAll(
                    dbContext.FileDocuments
                        .Where(x => documentIdsToSync.Contains(x.Id))
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.FaissSynced, true)),
                    saveFunc.Invoke(),
                    dbContext.SingleDocumentChunks
                        .AddRangeAsync(singleChunkEntity)
                );

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbResult(true);
            }
            catch(Exception ex)
            {
                new LoggerFactory().CreateLogger<FileCollectionFaissRepository>()
                    .LogError(ex, "Exception occured while saving file collection faiss");
                
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
    public enum FileCollectionFaissRepositorySaveMode
    {
        Create, Update
    }
}