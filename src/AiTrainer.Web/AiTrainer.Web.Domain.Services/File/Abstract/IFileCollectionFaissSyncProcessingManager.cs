using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSyncProcessingManager: IDomainService
{
    Task SyncUserFileCollectionFaissStore(Domain.Models.User currentUser, Guid? collectionId = null,
        CancellationToken cancellationToken = default);
}