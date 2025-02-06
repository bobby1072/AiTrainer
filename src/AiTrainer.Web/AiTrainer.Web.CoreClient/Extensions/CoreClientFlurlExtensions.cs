using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class CoreClientFlurlExtensions
{
    public static async Task<TReturn?> CoreClientExceptionHandling<TReturn, TLoggObject>(this Task<TReturn> coreClientRequest, ILogger<TLoggObject> logger,
        string opName) where TReturn: class 
    {
        try
        {
            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => coreClientRequest);
            
            logger.LogDebug("{OpName} took a total time of {TimeTaken}ms to complete", opName, timeTaken.Milliseconds);

            if (result is CoreResponse { IsSuccess: false } coreResponse)
            {
                logger.LogError("{OpName} Core request was unsuccessful with exception message of {ExMessage}",
                    opName,
                    coreResponse.ExceptionMessage);
            }
            
            return result;
        }
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "{NameOfOp} request failed with status code {StatusCode}",
                opName,
                ex.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{NameOfOp} request failed",
                opName);
            return null;
        }
    }
    public static IFlurlRequest WithCorrelationIdHeader(this IFlurlRequest url, Guid? correlationIdHeader = null)
    {
        return url.WithHeader(ApiConstants.CorrelationIdHeader, correlationIdHeader?.ToString() ?? Guid.NewGuid().ToString());
    }
    public static IFlurlRequest WithCorrelationIdHeader(this Url url, Guid? correlationIdHeader = null)
    {
        return url.WithHeader(ApiConstants.CorrelationIdHeader, correlationIdHeader?.ToString() ?? Guid.NewGuid().ToString());
    }
    public static IFlurlRequest WithCorrelationIdHeader(this string url, Guid? correlationIdHeader = null)
    {
        return url.WithHeader(ApiConstants.CorrelationIdHeader, correlationIdHeader?.ToString() ?? Guid.NewGuid().ToString());
    }
    public static IFlurlRequest WithAiTrainerCoreKeyHeader(this Url url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
    public static IFlurlRequest WithAiTrainerCoreKeyHeader(this IFlurlRequest url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
    public static IFlurlRequest WithAiTrainerCoreKeyHeader(this string url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
}