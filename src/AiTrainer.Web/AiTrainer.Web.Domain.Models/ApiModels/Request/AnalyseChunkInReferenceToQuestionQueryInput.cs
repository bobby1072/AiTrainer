using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models;

public sealed class AnalyseChunkInReferenceToQuestionQueryInput: ChatQueryInput
{
    public required string Question { get; init; }

    public override bool Equals(AnalyseChunkInReferenceToQuestionQueryInput? obj)
    {
        return Question == obj?.Question;
    }
}