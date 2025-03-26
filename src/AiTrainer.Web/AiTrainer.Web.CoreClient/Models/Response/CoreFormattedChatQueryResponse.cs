namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreFormattedChatQueryResponse : BaseCoreClientResponseData
{
    public required string Content { get; init; }
}