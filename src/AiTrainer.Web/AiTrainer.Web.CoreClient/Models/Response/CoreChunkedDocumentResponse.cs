
namespace AiTrainer.Web.CoreClient.Models.Response
{
    public sealed record CoreChunkedDocumentResponse
    {
        public required IReadOnlyCollection<CoreSingleChunkedDocument> DocumentChunks { get; init; }
    }
    public sealed record CoreSingleChunkedDocument
    {
        public required IReadOnlyCollection<string> ChunkedTexts { get; init; }
        public required Guid FileDocumentId { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
