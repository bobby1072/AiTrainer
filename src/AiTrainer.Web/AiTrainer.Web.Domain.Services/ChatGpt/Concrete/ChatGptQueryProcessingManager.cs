using System.Net;
using System.Reflection;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using AiTrainer.Web.Persistence.Utils;
using BT.Common.FastArray.Proto;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Concrete;

internal class ChatGptQueryProcessingManager: IChatGptQueryProcessingManager
{
    private readonly ILogger<ChatGptQueryProcessingManager> _logger;
    private readonly ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse> _chatFormattedQueryClient;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
    private readonly IValidator<ChatGptFormattedQueryInput> _chatGptFormattedQueryValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IValidator<AnalyseChunkInReferenceToQuestionQueryInput> _analyseChunkInReferenceToQuestionQueryValidator;
    private static readonly IReadOnlyCollection<PropertyInfo> _selfProperties = typeof(ChatGptQueryProcessingManager).GetProperties();
    public ChatGptQueryProcessingManager(ICoreClient<FormattedChatQueryBuilder, 
        CoreFormattedChatQueryResponse> chatFormattedQueryClient,
        IValidator<ChatGptFormattedQueryInput> chatGptFormattedQueryValidator,
        IValidator<AnalyseChunkInReferenceToQuestionQueryInput> analyseChunkInReferenceToQuestionQueryValidator,
        IFileCollectionFaissRepository fileCollectionFaissRepository,
        ILogger<ChatGptQueryProcessingManager> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _chatFormattedQueryClient = chatFormattedQueryClient;
        _chatGptFormattedQueryValidator = chatGptFormattedQueryValidator;
        _analyseChunkInReferenceToQuestionQueryValidator = analyseChunkInReferenceToQuestionQueryValidator;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> ChatGptFaissQuery(
        ChatGptFormattedQueryInput input,
        Domain.Models.User user,
        CancellationToken cancellationToken = default)
    {
        var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();
        
        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(ChatGptFaissQuery),
            correlationId
        );
        var validationResult = await _chatGptFormattedQueryValidator.ValidateAsync(input, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ApiException($"{nameof(ChatGptFormattedQueryInput)} is not valid", HttpStatusCode.BadRequest);
        }
        
        var foundFileCollection = await EntityFrameworkUtils.TryDbOperation(() => _fileCollectionFaissRepository.ByUserAndCollectionId((Guid)user.Id!,
            input.CollectionId), _logger);

        if (foundFileCollection is null)
        {
            throw new InvalidOperationException("Failed to retrieve file collection faiss");
        } 
        if (foundFileCollection.Data is null)
        {
            throw new ApiException($"No faiss store found for user: {user.Id} and collectionId: {input.CollectionId}", HttpStatusCode.BadRequest);
        }

        var foundSingleChunk = foundFileCollection.Data.SingleDocuments.Value.FastArrayFirstOrDefault(x => x.Id == input.ChunkId);
        
        if (foundSingleChunk is null)
        {
            throw new ApiException("No chunk with that id found.", HttpStatusCode.BadRequest);
        }

        var queryEnum = (DefinedQueryFormatsEnum)input.DefinedQueryFormatsEnum;

        string queryResult = queryEnum switch
        {
            DefinedQueryFormatsEnum.AnalyseChunkInReferenceToQuestion => await
                Query<AnalyseChunkInReferenceToQuestionQueryInput>(
                    input, 
                    x => FormattedChatQueryBuilder.BuildAnalyseChunkInReferenceToQuestionQueryFormat(foundSingleChunk.PageContent, x.Question), 
                    correlationId?.ToString(),
                    cancellationToken),
            _ => throw new ApiException($"Unsupported query format: {queryEnum}", HttpStatusCode.BadRequest)
        };

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(ChatGptFaissQuery),
            correlationId
        );
        
        return queryResult;
    }
    private async Task<string> Query<TQueryType>(
        ChatGptFormattedQueryInput input,
        Func<TQueryType, FormattedChatQueryBuilder> formattedQueryBuilderFactory,
        string? correlationId,
        CancellationToken cancellationToken)
        where TQueryType: ChatQueryInput
    {
        _logger.LogInformation("Attempting query: {QueryName} for correlationId: {CorrelationId}",
            nameof(TQueryType),
            correlationId
        );
        
        var parsedQueryInput =
            JsonSerializer.Deserialize<TQueryType>(input.InputJson, ApiConstants
                .DefaultCamelCaseSerializerOptions) ?? throw new JsonException($"Failed to deserialize {nameof(TQueryType)}");

        await ValidateQuery(parsedQueryInput, cancellationToken);
        
        var actualQueryResult = await _chatFormattedQueryClient.TryInvokeAsync(
            formattedQueryBuilderFactory.Invoke(parsedQueryInput),
            cancellationToken
        ) ?? throw new InvalidOperationException("Failed to retrieve query result");
        
        return actualQueryResult.Content;
    }

    private async Task ValidateQuery<TQueryType>(TQueryType queryInput, CancellationToken cancellationToken)
        where TQueryType : ChatQueryInput
    {
        var foundValidator = _selfProperties.FirstOrDefault(x => x.PropertyType == typeof(IValidator<TQueryType>))?.GetValue(this) as IValidator<TQueryType>;

        if (foundValidator is null)
        {
            return;
        }
        
        var validationResult = await foundValidator.ValidateAsync(queryInput, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ApiException($"{nameof(TQueryType)} is not valid", HttpStatusCode.BadRequest);
        }
    }
}