using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SharedFileCollectionSingleMemberEmailSaveInput: SharedFileCollectionSingleMemberSaveInput
{
    public const string EmailJsonName = "email";
    [JsonPropertyName(EmailJsonName)]
    public required string Email { get; init; }
}