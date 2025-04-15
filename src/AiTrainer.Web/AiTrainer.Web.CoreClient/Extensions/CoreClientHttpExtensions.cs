using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text.Json;
using AiTrainer.Web.Common;
using BT.Common.OperationTimer.Proto;
using BT.Common.Polly.Extensions;
using BT.Common.Polly.Models.Abstract;
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
}