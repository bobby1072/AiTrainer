using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SharedFileCollectionMemberSaveInput
{
    [JsonPropertyName("id")]
    public Guid? Id { get; init; }
    [JsonPropertyName("userId")]
    public required Guid UserId { get; set; }
    [JsonPropertyName("collectionId")]
    public required Guid CollectionId { get; set; }
    [JsonPropertyName("canViewDocuments")]
    public bool CanViewDocuments { get; set; }
    [JsonPropertyName("canDownloadDocuments")]
    public bool CanDownloadDocuments { get; set; }
    [JsonPropertyName("canCreateDocuments")]
    public bool CanCreateDocuments { get; set; }
    [JsonPropertyName("canRemoveDocuments")]
    public bool CanRemoveDocuments { get; set; }
}