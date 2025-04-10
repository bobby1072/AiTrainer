using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal class FileCollectionFaissRemoveDocumentsBackgroundJob : FileCollectionFaissBackgroundJob<IFileCollectionFaissRemoveDocumentsProcessingManager>
{
    public required IReadOnlyCollection<SingleDocumentChunk> DocumentsToRemove { get; init; }

    public override Task JobProcessToRun(IFileCollectionFaissRemoveDocumentsProcessingManager manager, CancellationToken ct)
    {
       return manager.RemoveDocumentsFromFaissStoreAndSaveIt(CollectionId, DocumentsToRemove, CurrentUser, ct);
    }
}