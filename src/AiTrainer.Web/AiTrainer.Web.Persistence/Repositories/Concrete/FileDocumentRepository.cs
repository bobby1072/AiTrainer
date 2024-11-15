using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AiTrainer.Web.Domain.Models.Partials;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileDocumentRepository : BaseRepository<FileDocumentEntity, Guid, FileDocument>, IFileDocumentRepository
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
        public async Task<DbGetManyResult<FileDocumentPartial>> GetTopLevelDocumentPartialsForUser(Guid userId, params string[] relationShips)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relationShips);


            var entities = await TimeAndLogDbOperation(() => setToQuery
                .Select(x => new { x.Id, x.CollectionId, x.DateCreated, x.FileName, x.FileType, x.UserId })
                .Where(x => x.CollectionId == null && x.UserId == userId)
                .ToArrayAsync(), nameof(GetTopLevelDocumentPartialsForUser), _entityType.Name);

            return new DbGetManyResult<FileDocumentPartial>(entities?.FastArraySelect(x => SelectDataToPartial(x)).ToArray());
        }
        public async Task<DbGetManyResult<FileDocumentPartial>> GetManyDocumentPartialsByCollectionId(Guid collectionId, Guid userId, params string[] relationShips)
        {
            await using var dbContext = await _contextFactory.CreateDbContextAsync();

            var setToQuery = AddRelationsToSet(dbContext.FileDocuments, relationShips);


            var entities = await TimeAndLogDbOperation(() => setToQuery
                .Select(x => new { x.Id, x.CollectionId, x.DateCreated, x.FileName, x.FileType, x.UserId })
                .Where(x => x.CollectionId == collectionId && x.UserId == userId)
                .ToArrayAsync(), nameof(GetManyDocumentPartialsByCollectionId), _entityType.Name);

            return new DbGetManyResult<FileDocumentPartial>(entities?.FastArraySelect(x => SelectDataToPartial(x)).ToArray());
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
                Id = x.Id
            };
        }
    }
}
