using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public record RequiredGuidIdInput
    {
        [Required]
        [JsonPropertyName("id")]
        public required Guid Id { get; init; }
    }
}
