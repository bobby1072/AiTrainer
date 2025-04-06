using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal class SingleDocumentChunkRepository: BaseRepository<SingleDocumentChunkEntity, Guid, SingleDocumentChunk>
{
    public SingleDocumentChunkRepository(
        IDbContextFactory<AiTrainerContext> dbContextFactory,
        ILogger<SingleDocumentChunkRepository> logger
    )
        : base(dbContextFactory, logger)
    {}

    protected override SingleDocumentChunkEntity RuntimeToEntity(SingleDocumentChunk runtimeObj)
    {
        return runtimeObj.ToEntity();
    }
}