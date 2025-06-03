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
using BT.Common.Helpers.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Concrete;

internal sealed class ChatGptQueryProcessingManager : IChatGptQueryProcessingManager
{
    private readonly ILogger<ChatGptQueryProcessingManager> _logger;
    private readonly ICoreClient<
        FormattedChatQueryBuilder,
        CoreFormattedChatQueryResponse
    > _chatFormattedQueryClient;
    private readonly IFileCollectionFaissRepository _fileCollectionFaissRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidator<ChatGptFormattedQueryInput> _chatGptFormattedQueryValidator;

    public ChatGptQueryProcessingManager(
        ICoreClient<
            FormattedChatQueryBuilder,
            CoreFormattedChatQueryResponse
        > chatFormattedQueryClient,
        IValidator<ChatGptFormattedQueryInput> chatGptFormattedQueryValidator,
        IFileCollectionFaissRepository fileCollectionFaissRepository,
        ILogger<ChatGptQueryProcessingManager> logger,
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _chatFormattedQueryClient = chatFormattedQueryClient;
        _chatGptFormattedQueryValidator = chatGptFormattedQueryValidator;
        _fileCollectionFaissRepository = fileCollectionFaissRepository;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> ChatGptFaissQuery(
        ChatGptFormattedQueryInput input,
        Domain.Models.User user,
        CancellationToken cancellationToken = default
    )
    {
        var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(ChatGptFaissQuery),
            correlationId
        );
        var validationResult = await _chatGptFormattedQueryValidator.ValidateAsync(
            input,
            cancellationToken
        );
        if (!validationResult.IsValid)
        {
            throw new ApiException(
                $"{nameof(ChatGptFormattedQueryInput)} is not valid",
                HttpStatusCode.BadRequest
            );
        }
        

        var queryEnum = (DefinedQueryFormatsEnum)input.DefinedQueryFormatsEnum;

        _logger.LogInformation(
            "Attempting query: {QueryName} for correlationId: {CorrelationId}",
            queryEnum.GetDisplayName(),
            correlationId
        );
        var queryResult = queryEnum switch
        {
            DefinedQueryFormatsEnum.AnalyseChunkInReferenceToQuestion =>
                await Query<AnalyseChunkInReferenceToQuestionQueryInput>(
                    input,
                    x => AnalyseChunkInReferenceToQuestionQueryInputToFormattedChatQueryBuilder(x, (Guid)user.Id!, input.CollectionId),
                    cancellationToken
                ),
            _ => throw new ApiException(
                $"Unsupported query format: {queryEnum}",
                HttpStatusCode.BadRequest
            ),
        };

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(ChatGptFaissQuery),
            correlationId
        );

        return queryResult;
    }

    private async Task<FormattedChatQueryBuilder> AnalyseChunkInReferenceToQuestionQueryInputToFormattedChatQueryBuilder(
        AnalyseChunkInReferenceToQuestionQueryInput input, Guid userId, Guid? collectionId)
    {
        var fileCollectionFaiss = await GetFileCollectionFaiss(userId, collectionId);
        
        var foundSingleChunk =
            fileCollectionFaiss.SingleDocuments.Value.FastArrayFirstOrDefault(y =>
                y.Id == input.ChunkId
            );

        if (foundSingleChunk is null)
        {
            throw new ApiException("No chunk with that id found.", HttpStatusCode.BadRequest);
        }

        _logger.LogInformation(
            "Single chunk being used in the query is id: {ChunkId} and file document id: {FileDocumentId}",
            foundSingleChunk.Id,
            foundSingleChunk.FileDocumentId
        );
        return FormattedChatQueryBuilder.BuildAnalyseChunkInReferenceToQuestionQueryFormat(
            foundSingleChunk.PageContent,
            input.Question
        );
    }
    private async Task<string> Query<TQueryType>(
        ChatGptFormattedQueryInput input,
        Func<TQueryType, Task<FormattedChatQueryBuilder>> formattedQueryBuilderFactory,
        CancellationToken cancellationToken
    )
        where TQueryType : ChatQueryInput
    {

        var parsedQueryInput =
            input.InputJson.Deserialize<TQueryType>() ?? throw new JsonException($"Failed to deserialize {typeof(TQueryType).Name}");

        await ValidateQuery(parsedQueryInput, cancellationToken);

        var actualQueryResult =
            await _chatFormattedQueryClient.TryInvokeAsync(
                await formattedQueryBuilderFactory.Invoke(parsedQueryInput),
                cancellationToken
            ) ?? throw new InvalidOperationException("Failed to retrieve query result");

        return actualQueryResult.Content;
    }

    private async Task ValidateQuery<TQueryType>(
        TQueryType queryInput,
        CancellationToken cancellationToken
    )
        where TQueryType : ChatQueryInput
    {
        var foundValidator = _serviceProvider.GetService<IValidator<TQueryType>>();

        if (foundValidator is null)
        {
            return;
        }

        var validationResult = await foundValidator.ValidateAsync(queryInput, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ApiException($"{typeof(TQueryType).Name} is not valid", HttpStatusCode.BadRequest);
        }
    }


    private async Task<FileCollectionFaiss> GetFileCollectionFaiss(Guid userId, Guid? collectionId)
    {
        var foundFileCollection = await EntityFrameworkUtils.TryDbOperation(
            () =>
                _fileCollectionFaissRepository.ByUserAndCollectionId(
                    userId,
                    collectionId
                ),
            _logger
        ) ?? throw new InvalidOperationException("Failed to retrieve file collection faiss");
        if (foundFileCollection.Data is null)
        {
            throw new ApiException(
                $"No faiss store found for user: {userId} and collectionId: {collectionId}",
                HttpStatusCode.BadRequest
            );
        }
        
        return foundFileCollection.Data;
    }
}
