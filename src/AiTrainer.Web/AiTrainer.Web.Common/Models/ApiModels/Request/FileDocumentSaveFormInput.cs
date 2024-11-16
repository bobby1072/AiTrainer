
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record FileDocumentSaveFormInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
        [JsonPropertyName("collectionId")]
        public Guid? CollectionId { get; init; }
        [JsonPropertyName("fileToCreate")]
        public required IFormFile FileToCreate { get; init; }
    }
}
