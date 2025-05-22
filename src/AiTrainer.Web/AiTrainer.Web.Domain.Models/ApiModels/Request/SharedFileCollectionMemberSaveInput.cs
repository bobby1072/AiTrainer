using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;


public sealed record SharedFileCollectionMemberSaveInput
{
    [JsonPropertyName("collectionId")]
    public required Guid CollectionId { get; init; }
    [JsonPropertyName("membersToShareTo")]
    public required IReadOnlyCollection<SharedFileCollectionSingleMemberSaveInput> MembersToShareTo { get; init; }
}
public sealed record SharedFileCollectionSingleMemberSaveInput
{
    [JsonPropertyName("userId")]
    public required Guid UserId { get; init; }
    [JsonPropertyName("canViewDocuments")]
    public bool CanViewDocuments { get; init; }
    [JsonPropertyName("canDownloadDocuments")]
    public bool CanDownloadDocuments { get; init; }
    [JsonPropertyName("canCreateDocuments")]
    public bool CanCreateDocuments { get; init; }
    [JsonPropertyName("canRemoveDocuments")]
    public bool CanRemoveDocuments { get; init; }
}