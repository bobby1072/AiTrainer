
namespace AiTrainer.Web.CoreClient.Models.Response
{
    public record CoreChunkedDocumentResponse
    {
        public required IReadOnlyCollection<SingleChunkedDocument> DocumentChunks { get; init; }
    }
    public record SingleChunkedDocument
    {
        public required IReadOnlyCollection<string> ChunkedTexts { get; init; }
        public required Guid FileDocumentId { get; init; }
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
