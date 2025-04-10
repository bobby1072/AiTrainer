using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

public record FileCollectionFaissSyncBackgroundJob: FileCollectionFaissBackgroundJob<IFileCollectionFaissSyncProcessingManager>
{
    internal override Task JobProcessToRun(IFileCollectionFaissSyncProcessingManager manager, CancellationToken ct)
    {
        return manager.SyncUserFileCollectionFaissStore(CurrentUser, CollectionId, ct);
    }
}