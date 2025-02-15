using System.Linq.Expressions;
using AiTrainer.Web.Domain.Services.File.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Models;

public record FileCollectionFaissSyncBackgroundJob
{
    public Guid? CollectionId { get; init; }
    public required Domain.Models.User User { get; init; }
    
    internal Expression<Func<IFileCollectionFaissSyncProcessingManager, CancellationToken, Task>> SyncProcess 
        => (service, ct) => service.SyncUserFileCollectionFaissStore(User, CollectionId, ct);
}