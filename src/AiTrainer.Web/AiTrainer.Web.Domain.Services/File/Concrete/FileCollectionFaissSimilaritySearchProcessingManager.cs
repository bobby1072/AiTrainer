using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissSimilaritySearchProcessingManager: IFileCollectionFaissSimilaritySearchProcessingManager
{
    private readonly ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse> _similaritySearchClient;
    private readonly IFileCollectionFaissRepository _faissRepository;
    private readonly ILogger<FileCollectionFaissSimilaritySearchProcessingManager> _logger;
    private readonly IFileCollectionRepository _fileCollectionRepository;

    public FileCollectionFaissSimilaritySearchProcessingManager(
        ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse> similaritySearchClient,
        IFileCollectionFaissRepository faissRepository,
        ILogger<FileCollectionFaissSimilaritySearchProcessingManager> logger,
        IFileCollectionRepository fileCollectionRepository
    )
    {
        _similaritySearchClient = similaritySearchClient;
        _faissRepository = faissRepository;
        _logger = logger;
        _fileCollectionRepository = fileCollectionRepository;
    }
}