
namespace AiTrainer.Web.CoreClient.Models.Request;

public record CoreCreateFaissStoreInput
{
    public required IReadOnlyCollection<CoreCreateFaissStoreInputDocument> Documents { get; init; }
}

public record CoreCreateFaissStoreInputDocument
{
    public required string PageContent { get; init; }
    public required Dictionary<string, string> Metadata { get; init; } = new();
}