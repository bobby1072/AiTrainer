namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreFormattedChatQueryInput: BaseCoreClientRequestData
{
    public required string SystemPromptMessage { get; init; }
    public required string HumanPromptMessage { get; init; }
    public Dictionary<string, string> ExtraInput { get; init; } = [];
}