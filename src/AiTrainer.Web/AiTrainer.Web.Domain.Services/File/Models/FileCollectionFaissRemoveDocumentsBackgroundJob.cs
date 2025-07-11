using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal sealed class FileCollectionFaissRemoveDocumentsBackgroundJob : FileCollectionFaissBackgroundJob
{
    public override Task ExecuteFaissJobAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var removeManager = sp.GetRequiredService<IFileCollectionFaissRemoveDocumentsProcessingManager>();
        
        return removeManager.RemoveDocumentsFromFaissStoreAndUpdateItAsync(CollectionId, CurrentUser, ct);
    }
}