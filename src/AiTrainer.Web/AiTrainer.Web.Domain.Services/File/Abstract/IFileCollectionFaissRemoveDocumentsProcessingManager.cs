namespace AiTrainer.Web.Domain.Services.File.Abstract;

internal interface IFileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissProcessingManager
{
    Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid? collectionId,
        Domain.Models.User currentUser,
        CancellationToken cancellationToken = default);
}