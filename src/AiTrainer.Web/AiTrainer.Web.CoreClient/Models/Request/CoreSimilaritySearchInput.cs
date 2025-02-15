using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreSimilaritySearchInput : BaseCoreClientRequestData
{
    [JsonIgnore] 
    public byte[] FileInput { get; init; } = [];
    [JsonPropertyName("docStore")]
    public required JsonDocument DocStore { get; init; }
    [JsonPropertyName("documentsToReturn")]
    public required int DocumentsToReturn { get; init; }
    [JsonPropertyName("question")]
    public required string Question { get; init; }
}