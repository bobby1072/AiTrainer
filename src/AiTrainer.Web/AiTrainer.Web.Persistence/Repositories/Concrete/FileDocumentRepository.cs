using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Partials;
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
    internal class FileDocumentRepository
        : BaseRepository<FileDocumentEntity, Guid, FileDocument>,
            IFileDocumentRepository
    {
        public FileDocumentRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<FileDocumentRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override FileDocumentEntity RuntimeToEntity(FileDocument runtimeObj)
        {
            return runtimeObj.ToEntity();
        }

        public async Task<DbSaveResult<FileDocument>> CreateOneWithMetaData(
            FileDocument document,
            FileDocumentMetaData metaData
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try{
                var documentEntity = document.ToEntity();

                await dbContext.FileDocuments.AddAsync(documentEntity);
                await dbContext.SaveChangesAsync();
                var newlySavedDoc = dbContext.FileDocuments.Local.FirstOrDefault();
                metaData.DocumentId = newlySavedDoc!.Id;

                var metaDataEntity = metaData.ToEntity();
                await dbContext.FileDocumentMetaData.AddAsync(metaDataEntity);
                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbSaveResult<FileDocument>([newlySavedDoc.ToModel()]);
            } catch (Exception e){
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<DbGetOneResult<FileDocument>> GetOne(Guid documentId, Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var entity = await TimeAndLogDbOperation(
                () =>
                    dbContext
                        .FileDocuments.Where(x => x.Id == documentId && x.UserId == userId)
                        .FirstOrDefaultAsync(),
                nameof(GetOne),
                _entityType.Name
            );

            return new DbGetOneResult<FileDocument>(entity?.ToModel());
        }

        public async Task<DbGetManyResult<FileDocumentPartial>> GetTopLevelDocumentPartialsForUser(
            Guid userId,
            params string[] relationShips
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relationShips);

            var entities = await TimeAndLogDbOperation(
                () =>
                    setToQuery
                        .Select(x => new
                        {
                            x.Id,
                            x.CollectionId,
                            x.DateCreated,
                            x.FileName,
                            x.FileDescription,
                            x.FileType,
                            x.Synced,
                            x.UserId,
                            x.MetaData
                        })
                        .Where(x => x.CollectionId == null && x.UserId == userId)
                        .ToArrayAsync(),
                nameof(GetTopLevelDocumentPartialsForUser),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocumentPartial>(
                entities?.FastArraySelect(x => SelectDataToPartial(x)).ToArray()
            );
        }

        public async Task<
            DbGetManyResult<FileDocumentPartial>
        > GetManyDocumentPartialsByCollectionId(
            Guid collectionId,
            Guid userId,
            params string[] relationShips
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relationShips);

            var entities = await TimeAndLogDbOperation(
                () =>
                    setToQuery
                        .Select(x => new
                        {
                            x.Id,
                            x.CollectionId,
                            x.DateCreated,
                            x.FileName,
                            x.FileType,
                            x.Synced,
                            x.FileDescription,
                            x.UserId,
                            x.MetaData
                        })
                        .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                        .ToArrayAsync(),
                nameof(GetManyDocumentPartialsByCollectionId),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocumentPartial>(
                entities?.FastArraySelect(x => SelectDataToPartial(x)).ToArray()
            );
        }

        public async Task<DbDeleteResult<Guid>> Delete(Guid documentId, Guid userId)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var foundDoc = await TimeAndLogDbOperation(
                () =>
                    dbContext
                        .FileDocuments.Where(x => x.Id == documentId && x.UserId == userId)
                        .ExecuteDeleteAsync(),
                nameof(Delete),
                _entityType.Name
            );

            return new DbDeleteResult<Guid>([documentId]);
        }

        private static FileDocumentPartial SelectDataToPartial(dynamic x)
        {
            return new FileDocumentPartial
            {
                CollectionId = x.CollectionId,
                DateCreated = x.DateCreated,
                FileName = x.FileName,
                FileType = (FileTypeEnum)x.FileType,
                UserId = x.UserId,
                Synced = x.Synced,
                FileDescription = x.FileDescription,
                Id = x.Id,
                MetaData = ((FileDocumentMetaDataEntity?)x.MetaData)?.ToModel()
            };
        }
    }
}
