
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissProcessingManager
{
    Task RemoveDocumentsFromFaissStoreAndSaveItAsync(Guid? collectionId,
        Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);

    Task<FileCollectionFaiss> RemoveDocumentsFromFaissStoreSafelyAsync(FileCollectionFaiss fileCollectionFaiss,
        IReadOnlyCollection<Guid> existingDocumentIds, CancellationToken cancellationToken = default);
}