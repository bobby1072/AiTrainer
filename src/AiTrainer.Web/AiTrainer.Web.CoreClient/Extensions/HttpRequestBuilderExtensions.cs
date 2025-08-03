using BT.Common.Http.Extensions;
using BT.Common.Http.Models;
using CommonApiConstants = BT.Common.Api.Helpers.ApiConstants;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class HttpRequestBuilderExtensions
{
    public static HttpRequestBuilder WithCoreApiKeyHeader(this HttpRequestBuilder requestBuilder, string apiKey)
    {
        requestBuilder.WithHeader(CoreClientConstants.ApiKeyHeader, apiKey);
        
        return requestBuilder;
    }

    public static HttpRequestBuilder WithCorrelationIdHeader(this HttpRequestBuilder requestBuilder, string? correlationId)
    {
        if (!string.IsNullOrEmpty(correlationId))
        {
            requestBuilder.WithHeader(CommonApiConstants.CorrelationIdHeader, correlationId);
        }
        
        
        return requestBuilder;
    }
}