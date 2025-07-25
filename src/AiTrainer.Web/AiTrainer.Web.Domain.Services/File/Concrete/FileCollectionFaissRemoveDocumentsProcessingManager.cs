﻿using System.Text.Json;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Helpers;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal sealed class FileCollectionFaissRemoveDocumentsProcessingManager
    : IFileCollectionFaissRemoveDocumentsProcessingManager
{
    private readonly ICoreClient<
        CoreRemoveDocumentsFromStoreInput,
        CoreFaissStoreResponse
    > _coreClient;
    private readonly ILogger<FileCollectionFaissRemoveDocumentsProcessingManager> _logger;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepo;
    private readonly IFileDocumentRepository _fileCollectionRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FileCollectionFaissRemoveDocumentsProcessingManager(
        IFileCollectionFaissRepository fileCollectionFaissRepo,
        IFileDocumentRepository fileCollectionRepo,
        ILogger<FileCollectionFaissRemoveDocumentsProcessingManager> logger,
        ICoreClient<CoreRemoveDocumentsFromStoreInput, CoreFaissStoreResponse> coreClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _fileCollectionFaissRepo = fileCollectionFaissRepo;
        _logger = logger;
        _coreClient = coreClient;
        _httpContextAccessor = httpContextAccessor;
        _fileCollectionRepo = fileCollectionRepo;
    }

    public async Task<FileCollectionFaiss> RemoveDocumentsFromFaissStoreSafelyAsync(
        FileCollectionFaiss fileCollectionFaiss,
        IReadOnlyCollection<Guid> existingDocumentIds,
        CancellationToken cancellationToken = default
    )
    {
        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(RemoveDocumentsFromFaissStoreSafelyAsync),
            correlationId
        );

        var analysedSingleChunkDocsToRemoveFromStore = fileCollectionFaiss
            .SingleDocuments.Value.FastArrayWhere(x =>
                !existingDocumentIds.Contains(x.FileDocumentId)
            )
            .ToArray();

        if (analysedSingleChunkDocsToRemoveFromStore.Length < 1)
        {
            _logger.LogInformation(
                "No documents to remove for correlationId {CorrelationId}",
                correlationId
            );
            return fileCollectionFaiss;
        }

        _logger.LogDebug(
            "Going to attempt to remove {@Documents} from faiss store for correlationId {CorrelationId}",
            analysedSingleChunkDocsToRemoveFromStore,
            correlationId
        );

        var deleteInCoreResult =
            await _coreClient.TryInvokeAsync(
                new CoreRemoveDocumentsFromStoreInput
                {
                    FileInput = fileCollectionFaiss.FaissIndex,
                    DocStore = fileCollectionFaiss.FaissJson,
                    DocumentIdsToRemove = analysedSingleChunkDocsToRemoveFromStore
                        .FastArraySelect(x => x.Id)
                        .ToArray(),
                },
                cancellationToken
            ) ?? throw new ApiException("Failed to delete chunks from the chosen faiss store");

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(RemoveDocumentsFromFaissStoreSafelyAsync),
            correlationId
        );

        return new FileCollectionFaiss
        {
            Id = fileCollectionFaiss.Id,
            UserId = fileCollectionFaiss.UserId,
            CollectionId = fileCollectionFaiss.CollectionId,
            FaissIndex = deleteInCoreResult.IndexFile,
            FaissJson = deleteInCoreResult.JsonDocStore,
            DateCreated = fileCollectionFaiss.DateCreated,
            DateModified = fileCollectionFaiss.DateModified,
        };
    }

    public async Task RemoveDocumentsFromFaissStoreAndUpdateItAsync(
        Guid? collectionId,
        Domain.Models.User currentUser,
        CancellationToken cancellationToken = default
    )
    {
        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(RemoveDocumentsFromFaissStoreAndUpdateItAsync),
            correlationId
        );

        var existingFaissStore =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _fileCollectionFaissRepo.ByUserAndCollectionId(
                        (Guid)currentUser.Id!,
                        collectionId
                    ),
                _logger
            ) ?? throw new ApiException("Failed to retrieve file collection faiss store");

        if (existingFaissStore.Data is null)
        {
            return;
        }

        var existingDocumentIds = await GetExistingDocumentIds((Guid)currentUser.Id!, collectionId);
        var allAnalysedChunksFromStore = existingFaissStore.Data.SingleDocuments.Value;

        var analysedSingleChunkDocsToRemoveFromStore = allAnalysedChunksFromStore
            .FastArrayWhere(x => !existingDocumentIds.Contains(x.FileDocumentId))
            .ToArray();

        if (analysedSingleChunkDocsToRemoveFromStore.Length < 1)
        {
            _logger.LogInformation(
                "No documents to remove for correlationId {CorrelationId}",
                correlationId
            );
            return;
        }

        await RemoveDirectlyFromStoreAndSave(
            existingFaissStore.Data.FaissIndex,
            existingFaissStore.Data.FaissJson,
            analysedSingleChunkDocsToRemoveFromStore.FastArraySelect(x => (Guid)x.Id!).ToArray(),
            (Guid)currentUser.Id!,
            collectionId,
            existingFaissStore.Data,
            correlationId?.ToString(),
            cancellationToken
        );

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(RemoveDocumentsFromFaissStoreAndUpdateItAsync),
            correlationId
        );
    }

    private async Task RemoveDirectlyFromStoreAndSave(
        byte[] faissIndex,
        JsonDocument jsonDocument,
        IReadOnlyCollection<Guid> documentIdsToRemove,
        Guid userId,
        Guid? collectionId,
        FileCollectionFaiss existingFaiss,
        string? correlationId,
        CancellationToken cancellationToken
    )
    {
        _logger.LogInformation(
            "Attempting to delete documents in core for correlationId {CorrelationId}",
            correlationId
        );

        _logger.LogDebug(
            "Attempting to delete documents with ids: {@DocumentsToDeleteIds}",
            documentIdsToRemove
        );

        var deleteInCoreResult =
            await _coreClient.TryInvokeAsync(
                new CoreRemoveDocumentsFromStoreInput
                {
                    FileInput = faissIndex,
                    DocStore = jsonDocument,
                    DocumentIdsToRemove = documentIdsToRemove,
                },
                cancellationToken
            ) ?? throw new ApiException("Failed to delete chunks from the chosen faiss store");

        var newFileCollectionFaiss = new FileCollectionFaiss
        {
            Id = existingFaiss.Id,
            CollectionId = collectionId,
            FaissIndex = deleteInCoreResult.IndexFile,
            FaissJson = deleteInCoreResult.JsonDocStore,
            UserId = userId,
            DateCreated = existingFaiss.DateCreated,
            DateModified = DateTime.UtcNow,
        };

        var analysedFileDocuments = newFileCollectionFaiss.SingleDocuments.Value;
        if (analysedFileDocuments.Count < 1)
        {
            _logger.LogInformation(
                "No documents left in faiss store for correlationId {CorrelationId}. Attempting to delete faiss store",
                correlationId
            );

            var storeUpdateResult = await EntityFrameworkUtils.TryDbOperation(
                () => _fileCollectionFaissRepo.Delete([(long)newFileCollectionFaiss.Id!])
            );

            if (storeUpdateResult?.Data is null)
            {
                throw new ApiException("Failed to delete faiss store");
            }
        }
        else
        {
            _logger.LogInformation(
                "Documents left in faissstore for correlationId {CorrelationId}. Attempting to save updated faiss store",
                correlationId
            );
            var storeUpdateResult = await EntityFrameworkUtils.TryDbOperation(
                () => _fileCollectionFaissRepo.Update([newFileCollectionFaiss])
            );

            if (storeUpdateResult?.IsSuccessful != true || storeUpdateResult?.Data is null)
            {
                throw new ApiException("Failed to save new updated faiss store");
            }
        }
    }

    private async Task<IReadOnlyCollection<Guid>> GetExistingDocumentIds(
        Guid currentUserId,
        Guid? collectionId
    )
    {
        var existingFileDocuments =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _fileCollectionRepo.GetManyDocumentPartialsByCollectionIdAndUserId(
                        currentUserId,
                        collectionId
                    ),
                _logger
            ) ?? throw new ApiException("Failed to retrieve file documents");

        var existingDocumentIds = existingFileDocuments
            .Data.FastArraySelect(x => (Guid)x.Id!)
            .ToHashSet();

        return existingDocumentIds;
    }
}
