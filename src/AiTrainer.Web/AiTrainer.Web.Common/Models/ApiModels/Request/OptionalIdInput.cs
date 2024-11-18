using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record OptionalIdInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
    }
}
