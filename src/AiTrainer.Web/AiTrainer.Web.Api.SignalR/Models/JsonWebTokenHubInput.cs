using System.Text.Json.Serialization;

namespace AiTrainer.Web.Api.SignalR.Models;

public record JsonWebTokenHubInput
{
    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; init; }
}