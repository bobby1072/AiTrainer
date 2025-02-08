using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunkInput: BaseCoreClientRequestData
    {
        [JsonPropertyName("documentText")]
        public required IReadOnlyCollection<string> DocumentText { get; init; }
    }
}
