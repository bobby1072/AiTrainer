using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SyncFaissStoreHubInput
{
    [JsonPropertyName("collectionId")]
    public Guid? CollectionId { get; init; }
}