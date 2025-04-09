using System.Net.Http.Headers;
using AiTrainer.Web.Common;

namespace AiTrainer.Web.CoreClient.Extensions;

public static class HttpClientExtensions
{
    public static void AddCorrelationIdHeader(this HttpContentHeaders headers, Guid? correlationId)
    {
        if (correlationId is not null)
        {
            headers.TryAddWithoutValidation(ApiConstants.CorrelationIdHeader, correlationId.ToString());
        }
    }

    public static void AddApiKeyHeader(this HttpContentHeaders headers, string apiKey)
    {
        headers.TryAddWithoutValidation(CoreClientConstants.ApiKeyHeader, apiKey);
    }
}