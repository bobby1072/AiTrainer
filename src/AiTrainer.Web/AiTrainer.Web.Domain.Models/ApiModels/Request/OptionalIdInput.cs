using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public record OptionalIdInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
    }
}
