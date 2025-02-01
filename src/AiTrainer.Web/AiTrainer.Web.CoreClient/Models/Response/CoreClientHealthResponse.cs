using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreClientHealthResponse : BaseCoreClientResponseData
{
    [JsonPropertyName("message")]
    public required string Message { get; init; } 
}