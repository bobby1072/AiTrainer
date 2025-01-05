using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record FileDocumentSaveFormInput
    {
        [JsonPropertyName("collectionId")]
        public Guid? CollectionId { get; init; }

        [JsonPropertyName("fileToCreate")]
        public required IFormFile FileToCreate { get; init; }

        [JsonPropertyName("fileDescription")]
        public string? FileDescription { get; init; }
    }
}
