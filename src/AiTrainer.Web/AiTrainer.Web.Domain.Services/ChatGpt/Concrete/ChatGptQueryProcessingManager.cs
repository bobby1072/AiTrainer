using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.ChatGpt.Abstract;
using AiTrainer.Web.Persistence.Repositories.Abstract;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Concrete;

internal class ChatGptQueryProcessingManager: IChatGptQueryProcessingManager
{
    private readonly ILogger<ChatGptQueryProcessingManager> _logger;
    private readonly ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse> _chatFormattedQueryClient;
    private readonly ISingleDocumentChunkRepository _singleDocumentChunkRepository;
    public ChatGptQueryProcessingManager(ICoreClient<FormattedChatQueryBuilder, 
        CoreFormattedChatQueryResponse> chatFormattedQueryClient,
        ILogger<ChatGptQueryProcessingManager> logger,
        ISingleDocumentChunkRepository singleDocumentChunkRepository)
    {
        _chatFormattedQueryClient = chatFormattedQueryClient;
        _logger = logger;
        _singleDocumentChunkRepository = singleDocumentChunkRepository;
    }

    public Task<string> AnalyseSingleDocumentChunkInReferenceToQuestionQuery(AnalyseSingleDocumentChunkInReferenceToQuestionInput input, Domain.Models.User user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}