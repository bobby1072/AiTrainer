using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Polly.Models.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientHealth : ICoreClient<CoreClientHealthResponse>
{
    private readonly ILogger<CoreClientHealth> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private static readonly PollyRetrySettings _pollyHealthRetrySettings =
        new()
        {
            TotalAttempts = 2,
            TimeoutInSeconds = 5,
            DelayBetweenAttemptsInSeconds = 1,
        };

    public CoreClientHealth(
        ILogger<CoreClientHealth> logger,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CoreClientHealthResponse?> TryInvokeAsync(
        CancellationToken cancellation = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext.GetCorrelationId();

            using var httpResult = await _httpClient.SendWithRetry(
                requestMessage =>
                {
                    requestMessage.Method = HttpMethod.Get;
                    requestMessage.RequestUri = new Uri(
                        $"{_aiTrainerCoreConfiguration.BaseEndpoint}/api/healthrouter"
                    );
                    requestMessage.Headers.AddApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey);
                    requestMessage.Headers.AddCorrelationIdHeader(correlationId);
                },
                _pollyHealthRetrySettings,
                _logger,
                nameof(CoreClientHealth),
                correlationId?.ToString(),
                cancellation
            );

            var finalResponse = await httpResult.Content.TryDeserializeJson<
                CoreResponse<CoreClientHealthResponse>
            >(ApiConstants.DefaultCamelCaseSerializerOptions, cancellation);

            return finalResponse?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientHealth),
                ex.Message
            );

            return null;
        }
    }
}
