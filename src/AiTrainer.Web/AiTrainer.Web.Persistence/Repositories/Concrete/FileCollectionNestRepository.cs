using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionNestRepository
        : BaseRepository<FileCollectionNestEntity, long, FileCollectionNest>
    {
        public FileCollectionNestRepository(
            IDbContextFactory<AiTrainerContext> dbContextFactory,
            ILogger<FileCollectionNestRepository> logger
        )
            : base(dbContextFactory, logger) { }

        protected override FileCollectionNestEntity RuntimeToEntity(FileCollectionNest runtimeObj)
        {
            return runtimeObj.ToEntity();
        }
    }
}
