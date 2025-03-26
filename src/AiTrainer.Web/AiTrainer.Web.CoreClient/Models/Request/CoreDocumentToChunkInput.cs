
namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record CoreDocumentToChunkInput : BaseCoreClientRequestData
    {
        public required IReadOnlyCollection<SingleDocumentToChunk> DocumentsToChunk { get; init; }
    }
    public record SingleDocumentToChunk
    {
        public required string DocumentText { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
