
namespace AiTrainer.Web.CoreClient.Models.Response
{
    public record CoreChunkedDocumentResponse
    {
        public required IReadOnlyCollection<CoreSingleChunkedDocument> DocumentChunks { get; init; }
    }
    public record CoreSingleChunkedDocument
    {
        public required IReadOnlyCollection<string> ChunkedTexts { get; init; }
        public required Guid FileDocumentId { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
