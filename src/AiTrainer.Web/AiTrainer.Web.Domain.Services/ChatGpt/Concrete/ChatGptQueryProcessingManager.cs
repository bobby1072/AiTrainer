using System.Net;
using System.Text.Json;
using AiTrainer.Web.Common.Exceptions;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Helpers;
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
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IServiceProvider _serviceProvider;

    public ChatGptQueryProcessingManager(
        ICoreClient<
            FormattedChatQueryBuilder,
            CoreFormattedChatQueryResponse
        > chatFormattedQueryClient,
        ILogger<ChatGptQueryProcessingManager> logger,
        IServiceProvider serviceProvider,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _chatFormattedQueryClient = chatFormattedQueryClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> ChatGptQuery<TQueryInput>(
        TQueryInput input,
        Domain.Models.User user,
        CancellationToken cancellationToken = default
    ) where TQueryInput : ChatQueryInput
    {
        var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

        _logger.LogInformation(
            "Entering {Action} for correlationId {CorrelationId}",
            nameof(ChatGptQuery),
            correlationId
        );

        string queryResult = input switch
        {
            EditFileDocumentQueryInput editFileDocumentQueryInput => await Query(editFileDocumentQueryInput,
                EditFileDocumentQueryInputToFormattedChatQueryBuilder, correlationId.ToString(), cancellationToken),
            AnalyseDocumentChunkInReferenceToQuestionQueryInput analyseQueryInput => await Query(analyseQueryInput,
                x => AnalyseChunkInReferenceToQuestionQueryInputToFormattedChatQueryBuilder(x, (Guid)user.Id!,
                    x.CollectionId), correlationId.ToString(), cancellationToken),
            _ => throw new ApiException("Unsupported query format", HttpStatusCode.BadRequest)
        };

        _logger.LogInformation(
            "Exiting {Action} for correlationId {CorrelationId}",
            nameof(ChatGptQuery),
            correlationId
        );
        
        return queryResult;
    }
    private async Task<string> Query<TQueryType>(
        TQueryType input,
        Func<TQueryType, Task<FormattedChatQueryBuilder>> formattedQueryBuilderFactory,
        string? correlationId,
        CancellationToken cancellationToken
    )
        where TQueryType : ChatQueryInput
    {

        await ValidateQuery(input, cancellationToken);

        var formattedQuery = await formattedQueryBuilderFactory.Invoke(input);
        
        _logger.LogInformation(
            "Attempting query: {QueryName} for correlationId: {CorrelationId}",
            formattedQuery.GetQueryName(),
            correlationId
        );
        var actualQueryResult =
            await _chatFormattedQueryClient.TryInvokeAsync(
                formattedQuery,
                cancellationToken
            ) ?? throw new InvalidOperationException("Failed to retrieve query result");

        return actualQueryResult.Content;
    }
    private async Task<FormattedChatQueryBuilder> EditFileDocumentQueryInputToFormattedChatQueryBuilder(EditFileDocumentQueryInput queryInput)
    {
        _logger.LogDebug("Querying file document: {@FileDocument}", new
        {
            FileDocumentId = queryInput.FileDocumentToChange.Id,
            queryInput.FileDocumentToChange.FileName,
            FileType = queryInput.FileDocumentToChange.FileType.GetDisplayName(),
            DateCreated = queryInput.FileDocumentToChange.DateCreated.ToUniversalTime(),
        });
        
        return FormattedChatQueryBuilder
            .BuildEditFileDocumentQueryFormat(queryInput.ChangeRequest, await FileHelper.GetTextFromTextFile(queryInput.FileDocumentToChange.FileData));
    }
    private async Task<FormattedChatQueryBuilder> AnalyseChunkInReferenceToQuestionQueryInputToFormattedChatQueryBuilder(
        AnalyseDocumentChunkInReferenceToQuestionQueryInput input, Guid userId, Guid? collectionId)
    {
        var fileCollectionFaiss = await GetFileCollectionFaiss(userId, collectionId);
        
        var foundSingleChunk =
            fileCollectionFaiss.SingleDocuments.Value.FastArrayFirstOrDefault(y =>
                y.Id == input.ChunkId
            );

        if (foundSingleChunk is null)
        {
            throw new ApiException("No chunk with that id found", HttpStatusCode.BadRequest);
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
        var foundFileFaissCollectionRepo = _serviceProvider.GetRequiredService<IFileCollectionFaissRepository>();
        
        var foundFileCollection = await EntityFrameworkUtils.TryDbOperation(
            () =>
                foundFileFaissCollectionRepo.ByUserAndCollectionId(
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
