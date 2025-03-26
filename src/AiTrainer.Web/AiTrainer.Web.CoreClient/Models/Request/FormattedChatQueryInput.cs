namespace AiTrainer.Web.CoreClient.Models.Request;

public record FormattedChatQueryInput: BaseCoreClientRequestData
{
    public required string SystemMessage { get; init; }
    public required string HumanMessage { get; init; }
    public Dictionary<string, string> ExtraInput { get; init; } = [];
}