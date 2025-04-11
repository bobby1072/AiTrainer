using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;
using Microsoft.Extensions.DependencyInjection;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal class FileCollectionFaissRemoveDocumentsBackgroundJob : FileCollectionFaissBackgroundJob
{
    public override Task ExecuteFaissJob(IServiceProvider sp, CancellationToken ct = default)
    {
        var removeManager = sp.GetRequiredService<IFileCollectionFaissRemoveDocumentsProcessingManager>();
        
        return removeManager.RemoveDocumentsFromFaissStoreAndSaveIt(CollectionId, CurrentUser, ct);
    }
}