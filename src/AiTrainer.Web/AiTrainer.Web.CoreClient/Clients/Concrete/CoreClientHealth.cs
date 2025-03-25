using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Extensions;
using BT.Common.Polly.Models.Concrete;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientHealth: ICoreClient<CoreClientHealthResponse>
{
    private readonly ILogger<CoreClientHealth> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISerializer _serialiser;

    public CoreClientHealth(
        ILogger<CoreClientHealth> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor,
        ISerializer serialiser
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
        _serialiser = serialiser;
    }
    public async Task<CoreClientHealthResponse?> TryInvokeAsync(CancellationToken cancellation = default)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("healthrouter")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .WithSerializer(_serialiser)
            .GetJsonAsync<CoreResponse<CoreClientHealthResponse>>(new PollyRetrySettings
            {
                TotalAttempts = 2,
                TimeoutInSeconds = 3,
                DelayBetweenAttemptsInSeconds = 1
            }, cancellation)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientHealth));
        
            
        return response?.Data;
    }

}