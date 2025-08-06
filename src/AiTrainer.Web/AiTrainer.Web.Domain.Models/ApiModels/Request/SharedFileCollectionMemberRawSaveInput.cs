using System.Text.Json;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SharedFileCollectionMemberRawSaveInput
{
    public required Guid CollectionId { get; init; }
    public required IReadOnlyCollection<JsonDocument> MembersToShareTo { get; init; }
}