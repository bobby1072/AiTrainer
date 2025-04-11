namespace AiTrainer.Web.CoreClient.Models.Request;

internal record CoreFormattedChatQueryInput
{
    public required string SystemPromptMessage { get; init; }
    public required string HumanPromptMessage { get; init; }
    public Dictionary<string, string> ExtraInput { get; init; } = [];
}