using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.File.Models;

public class FileCollectionFaissSyncBackgroundJob: FileCollectionFaissBackgroundJob
{
    public bool? RetryOverride { get; init; }
    internal override Task ExecuteFaissJobAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var syncManager = sp.GetRequiredService<IFileCollectionFaissSyncProcessingManager>();
        
        return syncManager.SyncUserFileCollectionFaissStore(CurrentUser, CollectionId, RetryOverride, ct);
    }
}