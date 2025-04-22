namespace AiTrainer.Web.Domain.Models.ApiModels.Request;

public sealed record AnalyseChunkInReferenceToQuestionQueryInput : ChatQueryInput
{
    public required string Question { get; init; }
}