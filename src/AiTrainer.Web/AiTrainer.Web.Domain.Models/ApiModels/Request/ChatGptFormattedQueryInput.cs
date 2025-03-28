namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public record ChatGptFormattedQueryInput
{
    public required string Question { get; init; }
    
    public required Guid ChunkId { get; init; }
}