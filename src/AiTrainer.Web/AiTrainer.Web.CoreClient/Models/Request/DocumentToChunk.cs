using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunk: BaseCoreClientRequestBody
    {
        [JsonPropertyName("documentText")]
        public required string DocumentText { get; init; }
    }
}
