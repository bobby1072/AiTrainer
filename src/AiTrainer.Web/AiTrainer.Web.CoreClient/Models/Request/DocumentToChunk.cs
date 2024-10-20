using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Request
{
    public record DocumentToChunk
    {
        [Required]
        [JsonPropertyName("documentText")]
        public string DocumentText { get; init; }

        public DocumentToChunk(string documentText)
        {
            DocumentText = documentText;
        }
    }
}
