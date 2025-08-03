using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record EditFileDocumentQueryInput: ChatQueryInput
{
    [JsonPropertyName("fileDocumentId")]
    public required Guid FileDocumentId { get; init; }
    [JsonPropertyName("changeRequest")]
    public required string ChangeRequest { get; init; }
}