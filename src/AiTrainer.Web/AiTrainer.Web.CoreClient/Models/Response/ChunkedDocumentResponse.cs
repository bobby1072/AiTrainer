using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    public record ChunkedDocumentResponse : BaseCoreClientResponseData
    {
        [JsonPropertyName("documentChunks")]
        public required IReadOnlyCollection<SingleChunkedDocument> DocumentChunks { get; init; }
    }
    public record SingleChunkedDocument
    {
        [JsonPropertyName("chunkedTexts")]
        public required IReadOnlyCollection<string> ChunkedTexts { get; init; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
