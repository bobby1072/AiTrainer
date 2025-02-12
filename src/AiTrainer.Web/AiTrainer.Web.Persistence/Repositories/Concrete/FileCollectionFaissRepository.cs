using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionFaissRepository : BaseRepository<FileCollectionFaissEntity, long, FileCollectionFaiss>, IFileCollectionFaissRepository
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

        public async Task<DbResult> SaveStoreAndSyncDocs(FileCollectionFaiss fileCollectionFaiss, IReadOnlyCollection<Guid> documentIdsToSync,
            FileCollectionFaissRepositorySaveMode saveMode)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = fileCollectionFaiss.ToEntity();
                Func<Task<EntityEntry<FileCollectionFaissEntity>>> saveFunc = async () => saveMode == FileCollectionFaissRepositorySaveMode.Create ? await dbContext.FileCollectionFaiss
                    .AddAsync(entity): dbContext.FileCollectionFaiss.Update(entity);
                await Task.WhenAll(
                    dbContext.FileDocuments
                        .Where(x => documentIdsToSync.Contains(x.Id))
                        .ExecuteUpdateAsync(x => x.SetProperty(y => y.FaissSynced, true)),
                    saveFunc.Invoke()
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
    }
    public enum FileCollectionFaissRepositorySaveMode
    {
        Create, Update
    } 
}