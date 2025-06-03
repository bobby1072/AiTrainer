using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public abstract record SharedFileCollectionSingleMemberSaveInput
{
    [JsonPropertyName("canViewDocuments")]
    public bool CanViewDocuments { get; init; }
    [JsonPropertyName("canDownloadDocuments")]
    public bool CanDownloadDocuments { get; init; }
    [JsonPropertyName("canCreateDocuments")]
    public bool CanCreateDocuments { get; init; }
    [JsonPropertyName("canRemoveDocuments")]
    public bool CanRemoveDocuments { get; init; }
    [JsonPropertyName("canSimilaritySearch")]
    public bool CanSimilaritySearch { get; init; }
}