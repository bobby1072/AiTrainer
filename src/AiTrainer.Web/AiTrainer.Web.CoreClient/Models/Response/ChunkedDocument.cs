using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    public record ChunkedDocument
    {
        [JsonPropertyName("documentChunks")]
        public IReadOnlyCollection<string> DocumentChunks { get; init; }
    }
}
