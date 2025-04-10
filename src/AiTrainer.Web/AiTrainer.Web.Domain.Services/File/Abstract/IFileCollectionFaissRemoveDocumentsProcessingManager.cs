using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissProcessingManager
{
    Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid? collectionId,
        IReadOnlyCollection<SingleDocumentChunk> documentsToRemove, Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);
}