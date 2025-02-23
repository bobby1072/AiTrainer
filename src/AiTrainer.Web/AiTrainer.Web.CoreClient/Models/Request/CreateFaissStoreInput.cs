using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CreateFaissStoreInput : BaseCoreClientRequestData
{
    [JsonPropertyName("documents")]
    public required IReadOnlyCollection<CreateFaissStoreInputDocument> Documents { get; init; }
}

public record CreateFaissStoreInputDocument
{
    [JsonPropertyName("pageContent")]
    public required string PageContent { get; init; }
    [JsonPropertyName("metadata")]
    public required Dictionary<string, string> Metadata { get; init; } = new();
}