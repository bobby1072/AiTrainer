using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.Domain.Models;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class FormattedChatQueryBuilderCoreExtensions
{
    public static CoreFormattedChatQueryInput ToCoreInput(this FormattedChatQueryBuilder request)
    {
        return new CoreFormattedChatQueryInput
        {
            HumanPromptMessage = request.HumanMessage,
            SystemPromptMessage = request.SystemMessage,
            ExtraInput = request.QueryParameters
        };
    }
}