using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissRemoveDocumentsProcessingManager: IFileCollectionFaissRemoveDocumentsProcessingManager
{
    private readonly ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse> _coreClient;
    private readonly ILogger<FileCollectionFaissRemoveDocumentsProcessingManager> _logger;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public FileCollectionFaissRemoveDocumentsProcessingManager(
            IFileCollectionFaissRepository fileCollectionFaissRepo,
            ILogger<FileCollectionFaissRemoveDocumentsProcessingManager> logger,
            ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse> coreClient,
            IHttpContextAccessor httpContextAccessor
        )
    {
        _fileCollectionFaissRepo= fileCollectionFaissRepo;
        _logger = logger;
        _coreClient = coreClient;
        _httpContextAccessor = httpContextAccessor;
    }


    public async Task RemoveDocumentsFromFaissStoreAndSaveIt(Guid? collectionId, IReadOnlyCollection<SingleDocumentChunk> documentsToRemove, Domain.Models.User currentUser, CancellationToken cancellationToken = default)
    {
        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(RemoveDocumentsFromFaissStoreAndSaveIt),
            correlationId
        );

        var existingFaissStore = await EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepo.ByUserAndCollectionId(
                    (Guid)currentUser.Id!,
                    collectionId
                ),
            _logger
        );

        if (existingFaissStore?.Data is null)
        {
            throw new ApiException("Failed to retrieve file collection faiss store");
        }

        var deleteInCoreResult = await _coreClient.TryInvokeAsync(new CoreRemoveDocumentsFromStoreInput
        {
            FileInput = existingFaissStore.Data.FaissIndex,
            JsonDocStore = existingFaissStore.Data.FaissJson,
            DocumentIdsToRemove = documentsToRemove.FastArraySelect(x => (Guid)x.Id!).ToArray(),
        }, cancellationToken) ?? throw new ApiException("Failed to delete chunks from the chosen faiss store");

        var storeUpdateResult = await EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepo.Update([new FileCollectionFaiss
                {
                    Id = existingFaissStore.Data.Id,
                    CollectionId = collectionId,
                    FaissIndex = deleteInCoreResult.IndexFile,
                    FaissJson = deleteInCoreResult.JsonDocStore,
                    UserId = (Guid)currentUser.Id!,
                }])
        );

        if (storeUpdateResult?.IsSuccessful != true || storeUpdateResult?.Data is null)
        {
            throw new ApiException("Failed to save new updated faiss store");
        }
    }
}