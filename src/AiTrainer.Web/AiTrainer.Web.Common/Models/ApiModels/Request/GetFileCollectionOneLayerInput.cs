using System.Text.Json.Serialization;

namespace AiTrainer.Web.Common.Models.ApiModels.Request
{
    public record GetFileCollectionOneLayerInput
    {
        [JsonPropertyName("collectionId")]
        public Guid? CollectionId { get; init; }
    }
}
