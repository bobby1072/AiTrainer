using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response
{
    internal record ChunkedDocument
    {
        [JsonPropertyName("documentChunks")]
        public IReadOnlyCollection<string> DocumentChunks { get; init; }
    }
}
