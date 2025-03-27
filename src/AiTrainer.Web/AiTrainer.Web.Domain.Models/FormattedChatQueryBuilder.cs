namespace AiTrainer.Web.Domain.Models;

public sealed class FormattedChatQueryBuilder
{
    public string SystemMessage { get; init; }
    public string HumanMessage { get; init; }
    public Dictionary<string, string> QueryParameters { get; init; }
    private FormattedChatQueryBuilder(
        string systemMessage,
        string humanMessage,
        Dictionary<string, string> queryParameters
    )
    {
        SystemMessage = systemMessage;
        HumanMessage = humanMessage;
        QueryParameters = queryParameters;
    }

    /// <summary>
    /// This query format can be used to analyse a section of text in reference to a question about said text.
    /// </summary>
    public static FormattedChatQueryBuilder BuildAnalysisChunkInReferenceToQuestionQueryFormat(string textChunk, string question) => new(
        "You need to analyse questions in reference to this section of text: {textChunk}",
        question,
        new Dictionary<string, string>
        {
            { "textChunk", textChunk },
        }
    );
}