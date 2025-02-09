using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Helpers;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.OperationTimer.Proto;
using BT.Common.Polly.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

public class FileCollectionFaissSyncProcessingManager: IFileCollectionFaissSyncProcessingManager
{
    private readonly ICoreClient<DocumentToChunkInput, ChunkedDocumentResponse> _documentChunkerClient;
    private readonly ICoreClient<CreateFaissStoreInput, FaissStoreResponse> _createFaissStoreService;
    private readonly ICoreClient<UpdateFaissStoreInput, FaissStoreResponse> _updateFaissStoreService;
    private readonly IUserProcessingManager _userProcessingManager;
    private readonly IFileCollectionRepository _fileCollectionRepository;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
    private readonly ILogger<FileCollectionFaissSyncProcessingManager> _logger;
    private readonly IFileDocumentRepository _fileDocumentRepository;
    private readonly FaissSyncRetrySettingsConfiguration _retrySettings;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    private int SyncAttemptCount { get; set; }
    
    public FileCollectionFaissSyncProcessingManager(
            ICoreClient<DocumentToChunkInput, ChunkedDocumentResponse> documentChunkerClient,
            ICoreClient<CreateFaissStoreInput, FaissStoreResponse> createFaissStoreService,
            ICoreClient<UpdateFaissStoreInput, FaissStoreResponse> updateFaissStoreService,
            IUserProcessingManager userProcessingManager,
            IFileCollectionRepository fileCollectionRepository,
            ILogger<FileCollectionFaissSyncProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IFileCollectionFaissRepository fileCollectionFaissRepository,
            IOptionsSnapshot<FaissSyncRetrySettingsConfiguration> retrySettings,
            IHttpContextAccessor? httpContextAccessor = null
        )
    {
        _documentChunkerClient = documentChunkerClient;
        _createFaissStoreService = createFaissStoreService;
        _updateFaissStoreService = updateFaissStoreService;
        _userProcessingManager = userProcessingManager;
        _fileCollectionRepository = fileCollectionRepository;
        _logger = logger;
        _retrySettings = retrySettings.Value;
        _fileDocumentRepository = fileDocumentRepository;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SyncUserCollectionFaissStore(Guid? collectionId = null, bool useRetry = false)
    {
        SyncAttemptCount = 0;
        
        var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(SyncUserCollectionFaissStore),
            correlationId
        );
        
        if (!useRetry)
        {
            var timeTakenForNoRetryOp = await OperationTimerUtils.TimeAsync(() => SyncUserCollectionFaissStoreInnerMethod(collectionId, correlationId));
            
            _logger.LogInformation("Faiss sync for collection {CollectionId} completed in {TimeTaken}ms for correlationId {CorrelationId}", 
                collectionId,
                timeTakenForNoRetryOp,
                correlationId
            );
            _logger.LogInformation(
                "Exiting {Action} for correlationId {CorrelationId}",
                nameof(SyncUserCollectionFaissStore),
                correlationId
            );
            return;
        }

        var retryPipeline = _retrySettings.ToPipeline();
        
        var timeTakenForRetryOp = await OperationTimerUtils.TimeAsync(() =>
            retryPipeline.ExecuteAsync(async ct => await SyncUserCollectionFaissStoreInnerMethod(collectionId, correlationId)));
        
        _logger.LogInformation("Faiss sync for collection {CollectionId} completed in {TimeTaken}ms after {AttemptCount} attempts for correlationId {CorrelationId}", 
            collectionId,
            timeTakenForRetryOp,
            SyncAttemptCount,
            correlationId
        );
        
        
        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(SyncUserCollectionFaissStore),
            correlationId
        );
    }

    private async Task SyncUserCollectionFaissStoreInnerMethod(Guid? collectionId, Guid? correlationId)
    {
        SyncAttemptCount++;
        
        
        var foundUser = await _userProcessingManager
            .TryGetUserFromCache(
                _httpContextAccessor?.HttpContext.GetAccessToken() ?? throw new ApiException(ExceptionConstants.NotAuthorized, HttpStatusCode.Unauthorized)
            ) ?? throw new ApiException(ExceptionConstants.NotAuthorized, HttpStatusCode.Unauthorized);
        
        await GetCollectionByIdAndAuth(collectionId, (Guid)foundUser.Id!);
        
        var unSyncedDocuments = await EntityFrameworkUtils.TryDbOperation(() => _fileDocumentRepository.GetDocumentsBySyncAndCollectionId(false, collectionId)) ?? throw new ApiException("Failed to retrieve file documents");
        if (unSyncedDocuments.Data.Count == 0)
        {
            _logger.LogInformation("File collection {CollectionId} has no unsynced documents therefore does not need to sync. correlationId {CorrelationId}",
                collectionId,
                correlationId
            );
            return;
        }

        var allUnsyncedDocumentTextJob = GetTextFromFileDocuments(unSyncedDocuments.Data, correlationId);
        var existingFaissStoreJob = EntityFrameworkUtils.TryDbOperation(() => _fileCollectionFaissRepository.GetOne(collectionId, nameof(FileCollectionFaissEntity.CollectionId))); 
        await Task.WhenAll(existingFaissStoreJob, allUnsyncedDocumentTextJob);

        var allUnsyncedDocumentText = await allUnsyncedDocumentTextJob;
        var existingFaissStore = await existingFaissStoreJob ?? throw new ApiException("Failed to retrieve file collection faiss store");
        
        var chunkedDocument = await _documentChunkerClient.TryInvokeAsync(new DocumentToChunkInput { DocumentText = allUnsyncedDocumentText }) ?? throw new ApiException("Failed to retrieve file collection faiss store");

        var createStoreInput = new CreateFaissStoreInput
        {
            Documents = chunkedDocument.DocumentChunks,
        };
        
        var storeToSave = existingFaissStore.Data is null ? await _createFaissStoreService.TryInvokeAsync(createStoreInput):
            await _updateFaissStoreService.TryInvokeAsync(new UpdateFaissStoreInput {DocStore = existingFaissStore.Data.FaissJson, FileInput = existingFaissStore.Data.FaissIndex, NewDocuments = createStoreInput});
        if (storeToSave is null)
        {
            throw new ApiException("Failed to build file collection faiss store in core");
        }

        var result = await EntityFrameworkUtils.TryDbOperation(() =>
            existingFaissStore.Data is null ? 
                _fileCollectionFaissRepository.Create([new FileCollectionFaiss{ CollectionId = collectionId, FaissIndex = storeToSave.IndexFile, FaissJson = storeToSave.JsonDocStore}]):
                _fileCollectionFaissRepository.Update([new FileCollectionFaiss{ CollectionId = collectionId, FaissIndex = storeToSave.IndexFile, FaissJson = storeToSave.JsonDocStore, Id = existingFaissStore.Data.Id}]));
        if (result?.IsSuccessful != true)
        {
            throw new ApiException("Failed to save file collection faiss store");
        }
    }
    private async Task<FileCollection> GetCollectionByIdAndAuth(Guid? collectionId, Guid userId)
    {
        var foundCollection = await EntityFrameworkUtils.TryDbOperation(() =>
            _fileCollectionRepository.GetOne(collectionId, nameof(FileCollectionEntity.Id)));
        if (foundCollection?.IsSuccessful is false or null || foundCollection.Data is null)
        {
            throw new ApiException("Failed to retrieve file collection");
        }

        if (foundCollection.Data.UserId != userId)
        {
            throw new ApiException(ExceptionConstants.NotAuthorized, HttpStatusCode.Unauthorized);
        }
        
        return foundCollection.Data;        
    }
    
    private async Task<IReadOnlyCollection<string>> GetTextFromFileDocuments(IReadOnlyCollection<FileDocument> fileDocuments, Guid? correlationId)
    {
        _logger.LogInformation("Getting text from the file documents for correlationId {CorrelationId}", correlationId);
        
        var getTextJobList = new List<Task<IReadOnlyCollection<string>>>();

        foreach (var doc in fileDocuments)
        {
            if (doc.FileType == FileTypeEnum.Pdf)
            {
                getTextJobList.Add(
                    FileHelper.GetTextFromPdfFile(doc.FileData)
                );
            }
            else if (doc.FileType == FileTypeEnum.Text)
            {
                Func<Task<IReadOnlyCollection<string>>> getTextFunc = async () => [await FileHelper.GetTextFromTextFile(doc.FileData)];
                getTextJobList.Add(getTextFunc.Invoke());
            }
        }
        
        var allResults = await Task.WhenAll(getTextJobList);
        return allResults.SelectMany(x => x).ToArray();
    }
}