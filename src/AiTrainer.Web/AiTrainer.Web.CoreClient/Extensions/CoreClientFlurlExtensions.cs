using Flurl;
using Flurl.Http;

namespace AiTrainer.Web.CoreClient.Extensions;

public static class CoreClientFlurlExtensions
{
    public static IFlurlRequest WithAiTrainerCoreKeyHeader(this Url url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
    public static IFlurlRequest WithAiTrainerCoreKeyHeader(this string url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
}