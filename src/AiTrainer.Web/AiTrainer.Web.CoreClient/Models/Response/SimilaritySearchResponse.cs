using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response;

public record SimilaritySearchResponse: BaseCoreClientResponseData
{
    [JsonPropertyName("items")]
    public required IReadOnlyCollection<SimilaritySearchResponseItem> Items { get; init; }
}

public record SimilaritySearchResponseItem
{
    [JsonPropertyName("pageContent")]
    public required string PageContent { get; init; }
    [JsonPropertyName("metadata")]
    public required Dictionary<string, string> Metadata { get; init; } = new();
}