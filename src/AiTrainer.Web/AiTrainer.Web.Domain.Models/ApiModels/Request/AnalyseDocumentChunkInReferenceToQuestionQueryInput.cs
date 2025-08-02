using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record AnalyseDocumentChunkInReferenceToQuestionQueryInput : ChatQueryInput
{
    [JsonPropertyName("question")]
    public required string Question { get; init; }
    [JsonPropertyName("chunkId")]
    public required Guid ChunkId { get; init; }
}