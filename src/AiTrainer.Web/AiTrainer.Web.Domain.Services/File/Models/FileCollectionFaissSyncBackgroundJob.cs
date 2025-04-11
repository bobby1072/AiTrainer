using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal class FileCollectionFaissSyncBackgroundJob: FileCollectionFaissBackgroundJob
{

    public override Task ExecuteFaissJobAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var syncManager = sp.GetRequiredService<IFileCollectionFaissSyncProcessingManager>();
        
        return syncManager.SyncUserFileCollectionFaissStore(CurrentUser, CollectionId, ct);
    }
}