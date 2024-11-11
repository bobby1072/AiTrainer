using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete
{
    internal class FileCollectionFaissRepository : BaseRepository<FileCollectionFaissEntity, long, FileCollectionFaiss>
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
    }
}