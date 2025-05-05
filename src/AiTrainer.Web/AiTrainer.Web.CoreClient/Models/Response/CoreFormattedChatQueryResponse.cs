namespace AiTrainer.Web.CoreClient.Models.Response;

public sealed record CoreFormattedChatQueryResponse
{
    public required string Content { get; init; }
}