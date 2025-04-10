using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal class FileCollectionFaissSyncBackgroundJob: FileCollectionFaissBackgroundJob<IFileCollectionFaissSyncProcessingManager>
{
    public override Task JobProcessToRun(IFileCollectionFaissSyncProcessingManager manager, CancellationToken ct)
    {
        return manager.SyncUserFileCollectionFaissStore(CurrentUser, CollectionId, ct);
    }
}