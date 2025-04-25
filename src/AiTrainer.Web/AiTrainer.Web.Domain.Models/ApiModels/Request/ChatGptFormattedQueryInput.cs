using System.Text.Json;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public record ChatGptFormattedQueryInput
{
    [JsonPropertyName("inputJson")]
    public required JsonDocument InputJson { get; init; }

    [JsonPropertyName("definedQueryFormatsEnum")]
    public required int DefinedQueryFormatsEnum { get; init; }
    [JsonPropertyName("collectionId")]
    public Guid? CollectionId { get; init; }
}
