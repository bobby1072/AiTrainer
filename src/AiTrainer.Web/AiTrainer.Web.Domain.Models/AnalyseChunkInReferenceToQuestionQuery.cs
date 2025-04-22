using System.Text.Json.Serialization;

namespace AiTrainer.Web.Domain.Models;

public sealed class AnalyseChunkInReferenceToQuestionQuery: ChatQuery<AnalyseChunkInReferenceToQuestionQuery>
{
    public required string Question { get; init; }

    public override bool Equals(AnalyseChunkInReferenceToQuestionQuery? obj)
    {
        return Question == obj?.Question;
    }
}