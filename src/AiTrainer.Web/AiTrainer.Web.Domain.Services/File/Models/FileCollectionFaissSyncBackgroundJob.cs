using System.Linq.Expressions;
using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

internal record FileCollectionFaissSyncBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User User { get; init; }
    
    public Expression<Func<IFileCollectionFaissSyncProcessingManager, CancellationToken, Task>> SyncProcess 
        => (service, ct) => service.SyncUserFileCollectionFaissStore(User, CollectionId, ct);
}