using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CreateFaissStoreInput : BaseCoreClientRequestBody
{
    [JsonPropertyName("documents")]
    public required IReadOnlyCollection<string> Documents { get; init; }
    [JsonPropertyName("metadata")]
    public Dictionary<string, string> Metadata { get; init; } = new();
}