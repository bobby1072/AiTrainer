using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissProcessingManager
{
    Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid? collectionId,
        IReadOnlyCollection<SingleDocumentChunk> documentsToRemove, Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);
}