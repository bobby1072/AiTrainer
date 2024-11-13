using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record FileCollectionSaveInput
    {
        [JsonPropertyName("id")]
        public Guid? Id { get; init; }
        [JsonPropertyName("parentId")]
        public Guid? ParentId { get; init; }
        [JsonPropertyName("collectionName")]
        public required string CollectionName { get; init; }
        [JsonPropertyName("dateCreated")]
        public DateTime? DateCreated { get; init; }
        [JsonPropertyName("dateModified")]
        public DateTime? DateModified { get; init; }
    }
}
