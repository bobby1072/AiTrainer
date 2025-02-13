using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Models;
using AiTrainer.Web.Persistence.Repositories.Concrete;

namespace AiTrainer.Web.Persistence.Repositories.Abstract;

public interface IFileCollectionFaissRepository: IRepository<FileCollectionFaissEntity, long, FileCollectionFaiss>
{
    Task<DbResult> DeleteDocumentAndStoreAndUnsyncDocuments(FileDocument documentToDelete);
    Task<DbResult> SaveStoreAndSyncDocs(FileCollectionFaiss fileCollectionFaiss,
        IReadOnlyCollection<Guid> documentIdsToSync,
        FileCollectionFaissRepositorySaveMode saveMode);
}