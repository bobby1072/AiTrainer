using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunk
    {
        [JsonPropertyName("documentText")]
        public required string DocumentText { get; init; }
    }
}
