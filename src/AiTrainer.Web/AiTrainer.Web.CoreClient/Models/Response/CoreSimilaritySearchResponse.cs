
namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreSimilaritySearchResponse
{
    public required IReadOnlyCollection<SimilaritySearchResponseItem> Items { get; init; }
}

public record SimilaritySearchResponseItem
{
    public required string PageContent { get; init; }

    public Dictionary<string, string> Metadata { get; init; } = new();
}
