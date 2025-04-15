using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using AiTrainer.Web.Common;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.OperationTimer.Proto;
using BT.Common.Polly.Extensions;
using BT.Common.Polly.Models.Abstract;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;

namespace AiTrainer.Web.CoreClient.Extensions;

internal static class CoreClientHttpExtensions
{
    public static async Task<HttpResponseMessage> SendWithRetry<TLoggerObject>(this HttpClient client, Action<HttpRequestMessage> buildRequest, IPollyRetrySettings retrySettings, ILogger<TLoggerObject>? logger = null, string? opName = null, string? correlationId = null, CancellationToken cancellationToken = default)
    {
        var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => client.SendWithRetry(buildRequest, retrySettings, cancellationToken));
        
        logger?.LogDebug("{OpName} took a total time of {TimeTaken}ms to complete for correlationId {CorrelationId}", opName, timeTaken.Milliseconds, correlationId);
        
        return result;
    }
    private static async Task<HttpResponseMessage> SendWithRetry(this HttpClient client, Action<HttpRequestMessage> buildRequest, IPollyRetrySettings retrySettings, CancellationToken cancellationToken = default)
    {
        var retryPipeline =  retrySettings.ToPipeline();

        var result = await retryPipeline.ExecuteAsync(async ct =>
        {
            using var httpRequest = new HttpRequestMessage();
            buildRequest.Invoke(httpRequest);
            
            var responseFromRoute = await client.SendAsync(httpRequest, ct);
            
            responseFromRoute.EnsureSuccessStatusCode();
            
            return responseFromRoute;
        }, cancellationToken);
        
        return result;
    }
    public static JsonContent CreateApplicationJson<T>(T param, JsonSerializerOptions? serializerOptions = null) where T : class
    {
        return JsonContent.Create(param, MediaTypeHeaderValue.Parse(MediaTypeNames.Application.Json), serializerOptions);
    }
    
    public static void AddCorrelationIdHeader(this HttpRequestHeaders headers, Guid? correlationId)
    {
        if (correlationId is not null)
        {
            headers.TryAddWithoutValidation(ApiConstants.CorrelationIdHeader, correlationId.ToString());
        }
    }

    public static void AddApiKeyHeader(this HttpRequestHeaders headers, string apiKey)
    {
        headers.TryAddWithoutValidation(CoreClientConstants.ApiKeyHeader, apiKey);
    }
    public static async Task<TReturn?> CoreClientExceptionHandling<TReturn, TLoggObject>(this Task<TReturn?> coreClientRequest, ILogger<TLoggObject> logger,
        string opName, string? correlationId = null) where TReturn: class 
    {
        try
        {
            var (timeTaken, result) = await OperationTimerUtils.TimeWithResultsAsync(() => coreClientRequest);
            
            logger.LogDebug("{OpName} took a total time of {TimeTaken}ms to complete for correlationId {CorrelationId}", opName, timeTaken.Milliseconds, correlationId);

            if (result is CoreResponse { IsSuccess: false } failedCoreResponse)
            {
                logger.LogError("{OpName} Core request was unsuccessful with exception message of {ExMessage}",
                    opName,
                    failedCoreResponse.ExceptionMessage);
            } 
            else if (result is CoreResponse { IsSuccess: true })
            {
                logger.LogInformation("Successfully completed call to core api for correlationId {CorrelationId}",
                    correlationId);
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