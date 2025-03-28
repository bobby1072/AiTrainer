using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using AiTrainer.Web.Domain.Models;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Concrete;

internal class ChatGptProcessingManager
{
    private readonly ILogger<ChatGptProcessingManager> _logger;
    private readonly ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse> _chatFormattedQueryClient;

    public ChatGptProcessingManager(ICoreClient<FormattedChatQueryBuilder, CoreFormattedChatQueryResponse> chatFormattedQueryClient)
    {
        _chatFormattedQueryClient = chatFormattedQueryClient;
    }
    
}