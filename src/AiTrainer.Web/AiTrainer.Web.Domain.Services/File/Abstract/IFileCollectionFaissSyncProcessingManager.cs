﻿namespace AiTrainer.Web.Domain.Services.File.Abstract;

public interface IFileCollectionFaissSyncProcessingManager: IFileCollectionFaissProcessingManager
{
    Task SyncUserFileCollectionFaissStore(
        Domain.Models.User currentUser,
        Guid? collectionId = null,
        bool? retryOverride = null,
        CancellationToken cancellationToken = default
    );
    
    Task TriggerSyncUserFileCollectionFaissStore(
        Domain.Models.User currentUser,
        Guid? collectionId = null,
        bool? retryOverride = null,
        CancellationToken cancellationToken = default
    );
}