using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;


public sealed record SharedFileCollectionMemberSaveInput 
{
    public const string CollectionIdJsonName = "collectionId";
    public const string MembersToShareToJsonName = "membersToShareTo";
    [JsonPropertyName(CollectionIdJsonName)]
    public required Guid CollectionId { get; init; }
    [JsonPropertyName(MembersToShareToJsonName)]
    public required IReadOnlyCollection<SharedFileCollectionSingleMemberSaveInput> MembersToShareTo { get; init; }
}