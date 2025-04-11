using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;

namespace AiTrainer.Web.Persistence.Repositories.Abstract;

public interface ISingleDocumentChunkRepository: IRepository<SingleDocumentChunkEntity, Guid, SingleDocumentChunk>
{
    Task<DbGetManyResult<SingleDocumentChunk>> FindManyChunks(IReadOnlyCollection<Guid> documentIds,
        params string[] relations);
}