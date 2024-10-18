using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunk
    {
        [JsonPropertyName("documentText")]
        public string DocumentText { get; init; }
    }
}
