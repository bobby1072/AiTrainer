using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request;

public sealed record CoreSimilaritySearchInput
{
    [JsonIgnore] public byte[] FileInput { get; init; } = [];
    public required JsonDocument DocStore { get; init; }
    public required int DocumentsToReturn { get; init; }
    public required string Question { get; init; }
}