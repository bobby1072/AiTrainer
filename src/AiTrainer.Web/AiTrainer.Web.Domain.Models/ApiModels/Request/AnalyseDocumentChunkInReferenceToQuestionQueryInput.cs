namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record AnalyseDocumentChunkInReferenceToQuestionQueryInput : ChatQueryInput
{
    public required string Question { get; init; }
    public required Guid ChunkId { get; init; }
    public Guid? CollectionId { get; init; }
}