using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

public record FileCollectionFaissRemoveDocumentsBackgroundJob : FileCollectionFaissBackgroundJob<IFileCollectionFaissRemoveDocumentsProcessingManager>
{
    public IReadOnlyCollection<SingleDocumentChunk> DocumentsToRemove { get; init; }

    internal override Task JobProcessToRun(IFileCollectionFaissRemoveDocumentsProcessingManager manager, CancellationToken ct)
    {
       return manager.RemoveDocumentsFromFaissStoreAndSaveIt(CollectionId, DocumentsToRemove, CurrentUser, ct);
    }
}