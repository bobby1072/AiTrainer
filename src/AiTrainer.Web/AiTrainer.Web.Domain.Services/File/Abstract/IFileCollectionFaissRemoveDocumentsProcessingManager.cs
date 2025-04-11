
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissProcessingManager
{
    Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid? collectionId,
        Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);

    Task RemoveDocumentsFromFaissStoreAndSaveIt(FileCollectionFaiss existingFaissStore,
        Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);
}