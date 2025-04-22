using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public record ChatGptFormattedQueryInput
{
    [JsonPropertyName("inputJson")]
    public required string InputJson { get; init; }
    [JsonPropertyName("definedQueryFormatsEnum")]
    public required int DefinedQueryFormatsEnum { get; init; }
    [JsonPropertyName("chunkId")]
    public required Guid ChunkId { get; init; }
    [JsonPropertyName("collectionId")]
    public required Guid? CollectionId { get; init; }
}