using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Contexts;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Extensions;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using BT.Common.FastArray.Proto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Persistence.Repositories.Concrete;

internal class SingleDocumentChunkRepository: BaseRepository<SingleDocumentChunkEntity, Guid, SingleDocumentChunk>, ISingleDocumentChunkRepository
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

    public async Task<DbGetManyResult<SingleDocumentChunk>> FindManyChunks(IReadOnlyCollection<Guid> documentIds, params string[] relations)
    {
        await using var dbContext = await _contextFactory.CreateDbContextAsync();
        var set = AddRelationsToSet(dbContext.SingleDocumentChunks, relations);
        var foundChunks = await TimeAndLogDbOperation(() => 
            set.Where(x => documentIds.Contains(x.Id)).ToArrayAsync(),
            nameof(FindManyChunks),
            nameof(SingleDocumentChunkEntity)
        );

        return new DbGetManyResult<SingleDocumentChunk>(foundChunks.FastArraySelect(x => x.ToModel()).ToArray());
    }
}