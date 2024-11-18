using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record RequiredIdInput
    {
        [JsonPropertyName("id")]
        public required Guid Id { get; init; }
    }
}
