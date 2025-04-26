
namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record CoreDocumentToChunkInput
    {
        public required IReadOnlyCollection<CoreSingleDocumentToChunk> DocumentsToChunk { get; init; }
    }
    public record CoreSingleDocumentToChunk
    {
        public required string DocumentText { get; init; }
        public required Guid FileDocumentId { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
