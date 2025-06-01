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
    internal sealed class FileDocumentRepository
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

        public async Task<DbGetManyResult<FileDocument>> GetDocumentsBySync(bool syncSate, Guid userId, Guid? collectionId = null, params string[] relations)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relations);
            var ent = await TimeAndLogDbOperation(() => setToQuery
                    .Where(x => x.FaissSynced == syncSate && x.CollectionId == collectionId && x.UserId == userId)
                    .ToArrayAsync(),
                nameof(GetDocumentsBySync),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocument>(ent.FastArraySelect(x => x.ToModel()).ToArray());
        }
        public override async Task<DbSaveResult<FileDocument>> Create(IReadOnlyCollection<FileDocument> entObj)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var documentEnts = entObj.FastArraySelect(RuntimeToEntity).ToArray();

                await Task.WhenAll(
                        UpdateFileColLastUpdate(dbContext.FileCollections,
                            documentEnts.FastArraySelect(x => x.UserId).ToArray(),
                            documentEnts.FastArraySelectWhere(x => x.CollectionId is not null, x => (Guid)x.CollectionId!).ToArray()),
                        dbContext.FileDocuments.AddRangeAsync(documentEnts)
                );

                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbSaveResult<FileDocument>(dbContext.FileDocuments.Local.FastArraySelect(x => x.ToModel()).ToArray());
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public override async Task<DbDeleteResult<FileDocument>> Delete(
            IReadOnlyCollection<FileDocument> entObj)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var documentEnts = entObj.FastArraySelect(RuntimeToEntity).ToArray();

                await UpdateFileColLastUpdate(dbContext.FileCollections,
                    documentEnts.FastArraySelect(x => x.UserId).ToArray(),
                    documentEnts.FastArraySelectWhere(x => x.CollectionId is not null, x => (Guid)x.CollectionId!)
                        .ToArray());

                dbContext.FileDocuments.RemoveRange(documentEnts);
                
                await dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return new DbDeleteResult<FileDocument> { Data = documentEnts.FastArraySelect(x => x.ToModel()).ToArray() };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<DbSaveResult<FileDocument>> Create(
            FileDocument document,
            FileDocumentMetaData metaData
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            await using var transaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var documentEntity = document.ToEntity();

                await dbContext.FileDocuments.AddAsync(documentEntity);
                await dbContext.SaveChangesAsync();
                var newlySavedDoc = dbContext.FileDocuments.Local.FirstOrDefault();
                metaData.DocumentId = newlySavedDoc!.Id;

                var metaDataEntity = metaData.ToEntity();
                if (document.CollectionId is Guid foundColId)
                {
                    await Task.WhenAll(
                        UpdateFileColLastUpdate(dbContext.FileCollections, [document.UserId], [foundColId]),
                        dbContext.FileDocumentMetaData.AddAsync(metaDataEntity).AsTask()
                    );
                }
                else
                {
                    await dbContext.FileDocumentMetaData.AddAsync(metaDataEntity);
                }


                await dbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return new DbSaveResult<FileDocument>([newlySavedDoc.ToModel()]);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
                            x.FaissSynced,
                            x.UserId,
                            x.MetaData,
                        })
                        .Where(x => x.CollectionId == null && x.UserId == userId)
                        .ToArrayAsync(),
                nameof(GetTopLevelDocumentPartialsForUser),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocumentPartial>(
                entities?.FastArraySelect(SelectDataToPartial).ToArray()
            );
        }

        public async Task<DbGetManyResult<FileDocument>> GetManyDocumentsByCollectionIdAndUserId(
            Guid userId,
            Guid? collectionId,
            params string[] relationShips
        )
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            
            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relationShips);
            
            var entities = await TimeAndLogDbOperation(
                () =>
                    setToQuery
                        .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                        .ToArrayAsync(),
                nameof(GetManyDocumentPartialsByCollectionIdAndUserId),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocument>(
                entities.FastArraySelect(x => x.ToModel()).ToArray()
            );
        }

        public async Task<DbGetManyResult<FileDocumentPartial>> GetManyDocumentPartialsByCollectionId(
            Guid collectionId,
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
                            x.FaissSynced,
                            x.FileDescription,
                            x.UserId,
                            x.MetaData,
                        })
                        .Where(x => x.CollectionId == collectionId)
                        .ToArrayAsync(),
                nameof(GetManyDocumentPartialsByCollectionIdAndUserId),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocumentPartial>(
                entities?.FastArraySelect(SelectDataToPartial).ToArray()
            );
        }
        public async Task<
            DbGetManyResult<FileDocumentPartial>
        > GetManyDocumentPartialsByCollectionIdAndUserId(
            Guid userId,
            Guid? collectionId,
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
                            x.FaissSynced,
                            x.FileDescription,
                            x.UserId,
                            x.MetaData,
                        })
                        .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                        .ToArrayAsync(),
                nameof(GetManyDocumentPartialsByCollectionIdAndUserId),
                _entityType.Name
            );

            return new DbGetManyResult<FileDocumentPartial>(
                entities?.FastArraySelect(SelectDataToPartial).ToArray()
            );
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
                FaissSynced = x.FaissSynced,
                FileDescription = x.FileDescription,
                Id = x.Id,
                MetaData = ((FileDocumentMetaDataEntity?)x.MetaData)?.ToModel(),
            };
        }
    }
}
