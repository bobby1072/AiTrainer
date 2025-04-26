using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal class GlobalFileCollectionConfigRepository: BaseRepository<GlobalFileCollectionConfigEntity, long, GlobalFileCollectionConfig>
{
    public GlobalFileCollectionConfigRepository(ILogger<GlobalFileCollectionConfigRepository> logger,
        IDbContextFactory<AiTrainerContext> contextFactory): base(contextFactory, logger)
        { }

    protected override GlobalFileCollectionConfigEntity RuntimeToEntity(GlobalFileCollectionConfig runtimeObj)
    {
        return runtimeObj.ToEntity();
    }
}