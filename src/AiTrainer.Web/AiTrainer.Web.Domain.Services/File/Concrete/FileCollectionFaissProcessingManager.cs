using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Services.User.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

public class FileCollectionFaissProcessingManager
{
    private readonly ICoreClient<CreateFaissStoreInput, FaissStoreResponse> _createFaissStoreService;
    private readonly ICoreClient<UpdateFaissStoreInput, FaissStoreResponse> _updateFaissStoreService;
    private readonly ICoreClient<SimilaritySearchInput, SimilaritySearchResponse> _similaritySearchService;
    private readonly IUserProcessingManager _userProcessingManager;
    private readonly IFileCollectionRepository _fileCollectionRepository;
    private readonly IRepository<FileCollectionFaissEntity, long, FileCollectionFaiss> _fileCollectionFaissRepository;
    private readonly ILogger<FileCollectionFaissProcessingManager> _logger;
    private readonly IFileDocumentRepository _fileDocumentRepository;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public FileCollectionFaissProcessingManager(
            ICoreClient<CreateFaissStoreInput, FaissStoreResponse> createFaissStoreService,
            ICoreClient<UpdateFaissStoreInput, FaissStoreResponse> updateFaissStoreService,
            ICoreClient<SimilaritySearchInput, SimilaritySearchResponse> similaritySearchService,
            IUserProcessingManager userProcessingManager,
            IFileCollectionRepository fileCollectionRepository,
            ILogger<FileCollectionFaissProcessingManager> logger,
            IFileDocumentRepository fileDocumentRepository,
            IRepository<FileCollectionFaissEntity, long, FileCollectionFaiss> fileCollectionFaissRepository,
            IHttpContextAccessor? httpContextAccessor = null
        )
    {
        _createFaissStoreService = createFaissStoreService;
        _updateFaissStoreService = updateFaissStoreService;
        _similaritySearchService = similaritySearchService;
        _userProcessingManager = userProcessingManager;
        _fileCollectionRepository = fileCollectionRepository;
        _logger = logger;
        _fileDocumentRepository = fileDocumentRepository;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task SyncCollectionFaissStore(Guid? collectionId = null)
    {
        var unSyncedDocuments = await _fileDocumentRepository.GetMany(false, nameof(FileDocumentEntity.FaissSynced));
        
        if (unSyncedDocuments?.IsSuccessful is false or null)
        {
            throw new ApiException("Failed to retrieve file documents");
        }
        if (unSyncedDocuments.Data.Count == 0)
        {
            return;
        }
    }
}