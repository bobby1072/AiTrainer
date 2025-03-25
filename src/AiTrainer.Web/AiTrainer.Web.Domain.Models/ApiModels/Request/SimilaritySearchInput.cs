using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public record SimilaritySearchInput
{
    [JsonPropertyName("collectionId")]
    public Guid? CollectionId { get; init; }

    [JsonPropertyName("documentsToReturn")]
    public required int DocumentsToReturn { get; init; }

    [JsonPropertyName("question")]
    public required string Question { get; init; }
}
