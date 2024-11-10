using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionRepository
        : BaseRepository<FileCollectionEntity, Guid, FileCollection>
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
    }
}
