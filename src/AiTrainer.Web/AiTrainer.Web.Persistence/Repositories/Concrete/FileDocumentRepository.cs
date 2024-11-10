using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileDocumentRepository : BaseRepository<FileDocumentEntity, Guid, FileDocument>
    {
        public FileDocumentRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<FileDocumentRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override FileDocumentEntity RuntimeToEntity(FileDocument runtimeObj)
        {
            var entity = new FileDocumentEntity
            {
                CollectionId = runtimeObj.CollectionId,
                DateCreated = runtimeObj.DateCreated,
                FileType = (int)runtimeObj.FileType,
                FileName = runtimeObj.FileName,
                FileData = runtimeObj.FileData,
            };

            if (runtimeObj.Id.HasValue)
            {
                entity.Id = runtimeObj.Id.Value;
            }

            return entity;
        }
    }
}
