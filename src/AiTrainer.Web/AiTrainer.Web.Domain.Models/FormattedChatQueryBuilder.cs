using BT.Common.Helpers.Extensions;

namespace AiTrainer.Web.Domain.Models;

public sealed class FormattedChatQueryBuilder: DomainModel<FormattedChatQueryBuilder>
{
    private readonly DefinedQueryFormats _formatType;
    public string SystemMessage { get; private init; }
    public string HumanMessage { get; private init; }
    public Dictionary<string, string> QueryParameters { get; private init; }
    private FormattedChatQueryBuilder(
        DefinedQueryFormats formatType,
        string systemMessage,
        string humanMessage,
        Dictionary<string, string> queryParameters
    )
    {
        _formatType = formatType;
        SystemMessage = systemMessage;
        HumanMessage = humanMessage;
        QueryParameters = queryParameters;
    }

    public override bool Equals(FormattedChatQueryBuilder? obj)
    {
        return obj?.SystemMessage == SystemMessage && obj?.HumanMessage == HumanMessage
            && obj?.QueryParameters.IsStringSequenceEqual(QueryParameters) == true;
    }
    public string GetQueryName() => _formatType.GetDisplayName();
    public bool IsSameQueryFormat(FormattedChatQueryBuilder formattedChatQueryBuilder) => formattedChatQueryBuilder._formatType == _formatType;

    /// <summary>
    /// This query format can be used to analyse a section of text in reference to a question about said text.
    /// </summary>
    public static FormattedChatQueryBuilder BuildAnalyseChunkInReferenceToQuestionQueryFormat(string textChunk, string question) => new(
        DefinedQueryFormats.AnalyseChunkInReferenceToQuestion,
        "You need to analyse questions in reference to this section of text: {textChunk}",
        question,
        new Dictionary<string, string>
        {
            { "textChunk", textChunk },
        }
    );

    private enum DefinedQueryFormats
    {
        AnalyseChunkInReferenceToQuestion
    }
}