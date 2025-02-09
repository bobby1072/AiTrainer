using System.Text.Json.Serialization;

namespace AiTrainer.Web.Api.SignalR.Models.Requests;

public record SyncFaissStoreHubInput
{
    [JsonPropertyName("collectionId")]
    public Guid? CollectionId { get; init; }
}