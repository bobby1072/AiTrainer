using System.Text.Json.Serialization;

namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreClientHealthResponse : BaseCoreClientResponseBody
{
    [JsonPropertyName("message")]
    public required string Message { get; init; } 
}