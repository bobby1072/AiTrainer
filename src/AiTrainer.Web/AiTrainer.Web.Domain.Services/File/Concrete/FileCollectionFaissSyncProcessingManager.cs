﻿using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Helpers;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.Extensions;
using AiTrainer.Web.Domain.Services.Concrete;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.File.Models;
using AiTrainer.Web.Domain.Services.Models;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Repositories.Concrete;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using BT.Common.OperationTimer.Proto;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal sealed class FileCollectionFaissSyncProcessingManager : IFileCollectionFaissSyncProcessingManager
{
    private readonly ICoreClient<
        CoreDocumentToChunkInput,
        CoreChunkedDocumentResponse
    > _documentChunkerClient;
    private readonly ICoreClient<
        CoreCreateFaissStoreInput,
        CoreFaissStoreResponse
    > _createFaissStoreService;
    private readonly ICoreClient<
        CoreUpdateFaissStoreInput,
        CoreFaissStoreResponse
    > _updateFaissStoreService;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
    private readonly ILogger<FileCollectionFaissSyncProcessingManager> _logger;
    private readonly IFileDocumentRepository _fileDocumentRepository;
    private readonly FaissSyncRetrySettingsConfiguration _retrySettings;
    private readonly IFileCollectionFaissRemoveDocumentsProcessingManager _fileCollectionFaissRemoveDocumentsProcessingManager;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly IFileCollectionFaissSyncBackgroundJobQueue _faissSyncBackgroundJobQueue;
    private readonly ILoggerFactory _loggerFactory;
    private int SyncAttemptCount { get; set; }

    public FileCollectionFaissSyncProcessingManager(
        ICoreClient<CoreDocumentToChunkInput, CoreChunkedDocumentResponse> documentChunkerClient,
        ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse> createFaissStoreService,
        ICoreClient<CoreUpdateFaissStoreInput, CoreFaissStoreResponse> updateFaissStoreService,
        ILogger<FileCollectionFaissSyncProcessingManager> logger,
        IFileDocumentRepository fileDocumentRepository,
        IFileCollectionFaissRepository fileCollectionFaissRepository,
        IOptions<FaissSyncRetrySettingsConfiguration> retrySettings,
        IFileCollectionFaissRemoveDocumentsProcessingManager fileCollectionFaissRemoveDocumentsProcessingManager,
        IFileCollectionFaissSyncBackgroundJobQueue faissSyncBackgroundJobQueue,
        ILoggerFactory loggerFactory,
        IHttpContextAccessor? httpContextAccessor = null
    )
    {
        _documentChunkerClient = documentChunkerClient;
        _createFaissStoreService = createFaissStoreService;
        _updateFaissStoreService = updateFaissStoreService;
        _logger = logger;
        _retrySettings = retrySettings.Value;
        _fileDocumentRepository = fileDocumentRepository;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _fileCollectionFaissRemoveDocumentsProcessingManager =
            fileCollectionFaissRemoveDocumentsProcessingManager;
        _faissSyncBackgroundJobQueue = faissSyncBackgroundJobQueue;
        _loggerFactory = loggerFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task TriggerSyncUserFileCollectionFaissStore(
        Domain.Models.User currentUser,
        Guid? collectionId = null,
        bool? retryOverride = null,
        CancellationToken cancellationToken = default
    )
    {
        await _faissSyncBackgroundJobQueue.EnqueueAsync(
            new FileCollectionFaissSyncBackgroundJob
            {
                CurrentUser = currentUser,
                CollectionId = collectionId,
                RetryOverride = retryOverride,
            },
            cancellationToken
        );
    }

    public async Task SyncUserFileCollectionFaissStore(
        Domain.Models.User currentUser,
        Guid? collectionId = null,
        bool? retryOverride = null,
        CancellationToken cancellationToken = default
    )
    {
        SyncAttemptCount = 0;

        var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(SyncUserFileCollectionFaissStore),
            correlationId
        );

        if (retryOverride == false || (retryOverride == null && !_retrySettings.UseRetry))
        {
            var timeTakenForNoRetryOp = await OperationTimerUtils.TimeAsync(
                () =>
                    SyncUserCollectionFaissStoreInnerMethod(
                        currentUser,
                        collectionId,
                        correlationId,
                        cancellationToken
                    )
            );

            _logger.LogInformation(
                "Faiss sync for collection {CollectionId} completed in {TimeTaken}ms for correlationId {CorrelationId}",
                collectionId,
                timeTakenForNoRetryOp,
                correlationId
            );
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(SyncUserFileCollectionFaissStore),
                correlationId
            );
            return;
        }

        var retryPipeline = _retrySettings.ToPipeline();

        var timeTakenForRetryOp = await OperationTimerUtils.TimeAsync(
            () =>
                retryPipeline.ExecuteAsync(
                    async ct =>
                        await SyncUserCollectionFaissStoreInnerMethod(
                            currentUser,
                            collectionId,
                            correlationId,
                            ct
                        ),
                    cancellationToken
                )
        );

        _logger.LogInformation(
            "Faiss sync for collection {CollectionId} completed in {TimeTaken}ms after {AttemptCount} attempts for correlationId {CorrelationId}",
            collectionId,
            timeTakenForRetryOp,
            SyncAttemptCount,
            correlationId
        );

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(SyncUserFileCollectionFaissStore),
            correlationId
        );
    }

    private async Task SyncUserCollectionFaissStoreInnerMethod(
        Domain.Models.User currentUser,
        Guid? collectionId,
        Guid? correlationId,
        CancellationToken cancelToken
    )
    {
        SyncAttemptCount++;

        var allExistingDocuments =
            await EntityFrameworkUtils.TryDbOperation(
                () =>
                    _fileDocumentRepository.GetManyDocumentsByCollectionIdAndUserId(
                        (Guid)currentUser.Id!,
                        collectionId,
                        nameof(FileDocumentEntity.MetaData)
                    ),
                _logger
            ) ?? throw new ApiException("Failed to retrieve file documents");

        var unSyncedDocuments = allExistingDocuments
            .Data.FastArrayWhere(x => x.FaissSynced == false)
            .ToArray();
        if (unSyncedDocuments.Length == 0)
        {
            _logger.LogInformation(
                "File collection {CollectionId} has no unsynced documents therefore does not need to sync. correlationId {CorrelationId}",
                collectionId,
                correlationId
            );
            return;
        }

        var existingDocumentIds = allExistingDocuments.Data.FastArraySelect(x => (Guid)x.Id!).ToArray();
        if (!_retrySettings.BatchSettings.UseBatching)
        {
            await SyncDocuments(
                unSyncedDocuments,
                existingDocumentIds,
                currentUser, 
                collectionId,
                collectionId,
                cancelToken
            );
        }
        else
        {
            var batchedExecutor = CreateBatchedAsyncOperationExecutor(
                existingDocumentIds,
                currentUser,
                collectionId,
                correlationId,
                cancelToken
            );

            await batchedExecutor.DoWorkAsync(unSyncedDocuments);
        }
    }
    
    private BatchedAsyncOperationExecutor<FileDocument> CreateBatchedAsyncOperationExecutor(
        IReadOnlyCollection<Guid> allDocumentIds,
        Domain.Models.User currentUser,
        Guid? collectionId,
        Guid? correlationId,
        CancellationToken cancelToken
    )
    {
        var opts = new BatchedAsyncOperationExecutorOptions<FileDocument>
        {
            BatchSize = _retrySettings.BatchSettings.BatchSize,
            BatchExecutionInterval = TimeSpan.FromSeconds(_retrySettings.BatchSettings.BatchExecutionIntervalInSeconds),
            SingleBatchHandler = (x, ct) =>
                SyncDocuments(x, allDocumentIds, currentUser, collectionId, correlationId, ct),
            CancellationToken = cancelToken,
            CorrelationId = correlationId?.ToString(),
            ReThrowOnBatchException = true,
        };
        
        return new BatchedAsyncOperationExecutor<FileDocument>(_loggerFactory.CreateLogger<BatchedAsyncOperationExecutor<FileDocument>>(), opts);
    }
    private async Task SyncDocuments(
        IReadOnlyCollection<FileDocument> documentsToSync,
        IReadOnlyCollection<Guid> allDocumentIds,
        Domain.Models.User currentUser,
        Guid? collectionId,
        Guid? correlationId,
        CancellationToken cancelToken)
    {
        var existingFaissStoreJob = EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepository.ByUserAndCollectionId(
                    (Guid)currentUser.Id!,
                    collectionId
                ),
            _logger
        );
        var allUnsyncedDocumentTextJob = GetTextFromFileDocuments(documentsToSync, correlationId);
        await Task.WhenAll(existingFaissStoreJob, allUnsyncedDocumentTextJob);

        var allUnsyncedDocumentText = await allUnsyncedDocumentTextJob;
        var existingFaissStore =
            await existingFaissStoreJob
            ?? throw new ApiException("Failed to retrieve file collection faiss store");
        var faissStoreBeforeUpdate = existingFaissStore.Data;
        if (existingFaissStore.Data is not null)
        {
            faissStoreBeforeUpdate =
                await _fileCollectionFaissRemoveDocumentsProcessingManager.RemoveDocumentsFromFaissStoreSafelyAsync(
                    existingFaissStore.Data,
                    allDocumentIds,
                    cancelToken
                );
        }

        var chunkedDocument =
            await _documentChunkerClient.TryInvokeAsync(
                new CoreDocumentToChunkInput
                {
                    DocumentsToChunk = allUnsyncedDocumentText
                        .FastArraySelect(x => new CoreSingleDocumentToChunk
                        {
                            DocumentText = x.DocText,
                            Metadata = x.Metadata,
                            FileDocumentId = x.DocId,
                        })
                        .ToArray(),
                },
                cancelToken
            ) ?? throw new ApiException("Failed to retrieve file collection faiss store");

        var storeToSave = await GetFaissStoreFromCoreApi(
            chunkedDocument,
            faissStoreBeforeUpdate,
            cancelToken
        );

        var faissStoreObjectToSave = CreateFileCollectionFaissStoreObject(
            storeToSave,
            (Guid)currentUser.Id!,
            collectionId,
            faissStoreBeforeUpdate
        );

        var result = await EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepository.SaveStoreAndSyncDocs(
                    faissStoreObjectToSave,
                    documentsToSync.FastArraySelect(x => (Guid)x.Id!).ToArray(),
                    existingFaissStore.Data is null
                        ? FileCollectionFaissRepositorySaveMode.Create
                        : FileCollectionFaissRepositorySaveMode.Update
                ),
            _logger
        );

        if (result?.IsSuccessful != true)
        {
            throw new ApiException("Failed to save file collection faiss store");
        }
    }
    private async Task<CoreFaissStoreResponse> GetFaissStoreFromCoreApi(
        CoreChunkedDocumentResponse coreChunkedDocument,
        FileCollectionFaiss? existingFaissStore,
        CancellationToken cancellationToken
    )
    {
        var createStoreInput = new CoreCreateFaissStoreInput
        {
            Documents = coreChunkedDocument
                .DocumentChunks.FastArraySelect(x =>
                    x.ChunkedTexts.FastArraySelect(y =>
                    {
                        x.Metadata.TryAdd(
                            nameof(CoreSingleChunkedDocument.FileDocumentId),
                            x.FileDocumentId.ToString()
                        );

                        return new CoreCreateFaissStoreInputDocument
                        {
                            PageContent = y,
                            Metadata = x.Metadata,
                        };
                    })
                )
                .SelectMany(x => x)
                .ToArray(),
        };

        var storeToSave = existingFaissStore is null
            ? await _createFaissStoreService.TryInvokeAsync(createStoreInput, cancellationToken)
            : await _updateFaissStoreService.TryInvokeAsync(
                new CoreUpdateFaissStoreInput
                {
                    DocStore = existingFaissStore.FaissJson,
                    FileInput = existingFaissStore.FaissIndex,
                    NewDocuments = createStoreInput,
                },
                cancellationToken
            );

        if (storeToSave is null)
        {
            throw new ApiException("Failed to build file collection faiss store in core");
        }

        return storeToSave;
    }

    private async Task<
        IReadOnlyCollection<(string DocText, Guid DocId, Dictionary<string, string> Metadata)>
    > GetTextFromFileDocuments(IReadOnlyCollection<FileDocument> fileDocuments, Guid? correlationId)
    {
        _logger.LogInformation(
            "Getting text from the file documents for correlationId {CorrelationId}",
            correlationId
        );

        var getTextJobList =
            new List<
                Task<
                    IReadOnlyCollection<(
                        string DocText,
                        Guid DocId,
                        Dictionary<string, string> Metadata
                    )>
                >
            >();

        foreach (var doc in fileDocuments)
        {
            if (doc.FileType == FileTypeEnum.Pdf)
            {
                async Task<
                    IReadOnlyCollection<(
                        string DocText,
                        Guid DocId,
                        Dictionary<string, string> Metadata
                    )>
                > GetFileTextAndMeta()
                {
                    var fileResult = await Task.Run(
                        () => FileHelper.GetTextFromPdfFile(doc.FileData)
                    );
                    return fileResult
                        .FastArraySelect(x => (x, (Guid)doc.Id!, doc.ToMetaDictionary()))
                        .ToArray();
                }
                getTextJobList.Add(GetFileTextAndMeta());
            }
            else if (doc.FileType == FileTypeEnum.Text)
            {
                async Task<
                    IReadOnlyCollection<(
                        string DocText,
                        Guid DocId,
                        Dictionary<string, string> Metadata
                    )>
                > GetTextFunc() =>
                    [
                        (
                            await Task.Run(() => FileHelper.GetTextFromTextFile(doc.FileData)),
                            (Guid)doc.Id!,
                            doc.ToMetaDictionary()
                        ),
                    ];
                getTextJobList.Add(GetTextFunc());
            }
        }

        var allResults = await Task.WhenAll(getTextJobList);
        return allResults.SelectMany(x => x).ToArray();
    }
    private static FileCollectionFaiss CreateFileCollectionFaissStoreObject(
        CoreFaissStoreResponse storeToSave,
        Guid userId,
        Guid? collectionId,
        FileCollectionFaiss? existingEntry
    )
    {
        var newFaiss = new FileCollectionFaiss
        {
            CollectionId = collectionId,
            FaissIndex = storeToSave.IndexFile,
            UserId = userId,
            FaissJson = storeToSave.JsonDocStore,
            DateCreated = existingEntry?.DateCreated ?? DateTime.UtcNow,
            DateModified = existingEntry?.DateModified ?? DateTime.UtcNow,
        };

        if (existingEntry is not null)
        {
            newFaiss.Id = existingEntry.Id;
        }

        return newFaiss;
    }
}
