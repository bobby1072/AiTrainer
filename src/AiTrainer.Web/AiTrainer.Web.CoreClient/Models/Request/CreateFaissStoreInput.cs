
namespace AiTrainer.Web.CoreClient.Models.Request;

public record CreateFaissStoreInput : BaseCoreClientRequestData
{
    public required IReadOnlyCollection<CreateFaissStoreInputDocument> Documents { get; init; }
}

public record CreateFaissStoreInputDocument
{
    public required string PageContent { get; init; }
    public required Dictionary<string, string> Metadata { get; init; } = new();
}