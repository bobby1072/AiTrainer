using System.Net;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.ApiModels.Request;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Services.File.Abstract;
using AiTrainer.Web.Persistence.Entities;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.File.Concrete;

internal class FileCollectionFaissSimilaritySearchProcessingManager : IFileCollectionFaissSimilaritySearchProcessingManager
{
    private readonly ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse> _similaritySearchClient;
    private readonly ILogger<FileCollectionFaissSimilaritySearchProcessingManager> _logger;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
    private readonly IValidator<SimilaritySearchInput> _inputValidator;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    public FileCollectionFaissSimilaritySearchProcessingManager(
        ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse> similaritySearchClient,
        ILogger<FileCollectionFaissSimilaritySearchProcessingManager> logger,
        IFileCollectionFaissRepository fileCollectionFaissRepository,
        IValidator<SimilaritySearchInput> inputValidator,
        IHttpContextAccessor? httpContextAccessor = null
    )
    {
        _similaritySearchClient = similaritySearchClient;
        _logger = logger;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _inputValidator = inputValidator;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SimilaritySearchCoreResponse> SimilaritySearch(SimilaritySearchInput input, Domain.Models.User currentUser)
    {
        var correlationId = _httpContextAccessor?.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(SimilaritySearch),
            correlationId
        );
        var validationResult = await _inputValidator.ValidateAsync(input);
        if (!validationResult.IsValid)
        {
            throw new ApiException("Invalid input for similarity search");
        }

        var existingFaissStore = await EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepository.ByUserAndCollectionId(
                    (Guid)currentUser.Id!,
                    input.CollectionId
                ),
            _logger
        );
        if (existingFaissStore?.Data?.UserId != (Guid)currentUser.Id!)
        {
            throw new ApiException(ExceptionConstants.Unauthorized, HttpStatusCode.Unauthorized);
        }


        _logger.LogInformation("Attempting to ask question of {Question} for collectionId {CollectionId} and correlationId {CorrelationId}",
            input.Question,
            input.CollectionId,
            correlationId);

        var result = await _similaritySearchClient.TryInvokeAsync(new CoreSimilaritySearchInput
        {
            Question = input.Question,
            FileInput = existingFaissStore.Data.FaissIndex,
            DocStore = existingFaissStore.Data.FaissJson,
            DocumentsToReturn = input.DocumentsToReturn,
        }) ?? throw new ApiException();

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(SimilaritySearch),
            correlationId
        );

        return result;
    }
}