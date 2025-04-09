using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IRemoveDocumentsFileCollectionFaissProcessingManager: IDomainService
{
    Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid collectionId,
        IReadOnlyCollection<SingleDocumentChunk> documentsToRemove, Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);
}