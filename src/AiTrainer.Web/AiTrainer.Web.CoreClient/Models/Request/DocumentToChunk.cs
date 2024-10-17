using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    internal record DocumentToChunk
    {
        [JsonPropertyName("documentText")]
        public string DocumentText { get; init; }
    }
}
