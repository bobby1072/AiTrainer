using AiTrainer.Web.Domain.Models.ApiModels.Request;

namespace AiTrainer.Web.Domain.Services.ChatGpt.Abstract;

public interface IChatGptQueryProcessingManager
{
    Task<string> ChatGptFaissQuery(
        ChatGptFormattedQueryInput input,
        Domain.Models.User user,
        CancellationToken cancellationToken = default);
}