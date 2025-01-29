using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CreateFaissStoreInput : BaseCoreClientRequestBody
{
    [JsonPropertyName("documents")]
    public required IReadOnlyCollection<string> Documents { get; init; }
}