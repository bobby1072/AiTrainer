namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreFormattedChatQueryResponse
{
    public required string Content { get; init; }
}