namespace AiTrainer.Web.CoreClient.Models.Response;

public record CoreClientHealthResponse : BaseCoreClientResponseData
{
    public required string Message { get; init; } 
}