using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunkInput : BaseCoreClientRequestData
    {
        [JsonPropertyName("documentsToChunk")]
        public required IReadOnlyCollection<SingleDocumentToChunk> DocumentsToChunk { get; init; }
    }
    public record SingleDocumentToChunk
    {
        [JsonPropertyName("documentText")]
        public required string DocumentText { get; init; }
        [JsonPropertyName("metadata")]
        public Dictionary<string, string> Metadata { get; init; } = [];
    }
}
