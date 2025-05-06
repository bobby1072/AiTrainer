using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request
{
    public sealed record FileDocumentSaveFormInput
    {
        [JsonPropertyName("collectionId")]
        public Guid? CollectionId { get; init; }

        [JsonPropertyName("fileToCreate")]
        public required IFormFile FileToCreate { get; init; }

        [JsonPropertyName("fileDescription")]
        public string? FileDescription { get; init; }
    }
}
