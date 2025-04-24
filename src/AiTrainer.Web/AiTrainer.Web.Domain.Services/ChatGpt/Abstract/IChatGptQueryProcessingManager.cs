using AiTrainer.Web.Domain.Models.ApiModels.Request;
using AiTrainer.Web.Domain.Services.Abstract;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Abstract;

public interface IChatGptQueryProcessingManager: IDomainProcessingManager
{
    Task<string> ChunkChatGptFaissQuery(
        ChatGptFormattedQueryInput input,
        Domain.Models.User user,
        CancellationToken cancellationToken = default);
}