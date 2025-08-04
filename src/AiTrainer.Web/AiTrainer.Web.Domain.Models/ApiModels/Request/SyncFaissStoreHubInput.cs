namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SyncFaissStoreHubInput
{
    public Guid? CollectionId { get; init; }
}