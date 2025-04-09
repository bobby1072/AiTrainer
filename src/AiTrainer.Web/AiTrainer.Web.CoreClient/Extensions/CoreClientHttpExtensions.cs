using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class CoreClientHttpExtensions
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
    public static async Task<TReturn?> CoreClientExceptionHandling<TReturn, TLoggObject>(this Task<TReturn?> coreClientRequest, ILogger<TLoggObject> logger,
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

    public static Task<T?> TryDeserializeJson<T>(this HttpContent content, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return content.ReadFromJsonAsync<T>(options, cancellationToken);
        }
        catch
        {
            return Task.FromResult((T?)default);
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
    public static IFlurlRequest WithAiTrainerCoreApiKeyHeader(this Url url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
    public static IFlurlRequest WithAiTrainerCoreApiKeyHeader(this IFlurlRequest url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
    public static IFlurlRequest WithAiTrainerCoreApiKeyHeader(this string url, string coreKey)
    {
        return url.WithHeader(CoreClientConstants.ApiKeyHeader, coreKey);
    }
}