using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record SharedFileCollectionSingleMemberUserIdSaveInput: SharedFileCollectionSingleMemberSaveInput
{
    public const string UserIdJsonName = "userId";
    [JsonPropertyName(UserIdJsonName)]
    public required Guid UserId { get; init; }
}