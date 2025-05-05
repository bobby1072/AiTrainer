
namespace AiTrainer.Web.CoreClient.Models.Response;

public sealed record CoreSimilaritySearchResponse
{
    public required IReadOnlyCollection<SimilaritySearchResponseItem> Items { get; init; }
}

public sealed record SimilaritySearchResponseItem
{
    public required string PageContent { get; init; }

    public Dictionary<string, string> Metadata { get; init; } = new();
}
