using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    public record ChunkedDocument
    {
        [JsonPropertyName("documentChunks")]
        public required IReadOnlyCollection<string> DocumentChunks { get; init; }
    }
}
