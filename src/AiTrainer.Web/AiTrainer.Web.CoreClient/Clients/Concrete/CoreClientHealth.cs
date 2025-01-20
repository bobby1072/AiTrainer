using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientHealth: ICoreClient<CoreClientHealthResponse>
{
    private readonly ILogger<CoreClientHealth> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;

    public CoreClientHealth(
        ILogger<CoreClientHealth> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
    }
    public async Task<CoreClientHealthResponse?> TryInvokeAsync()
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("healthrouter")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .GetJsonAsync<CoreResponse<CoreClientHealthResponse>>(_aiTrainerCoreConfiguration)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientHealth));
        
            
        return response?.Data;
    }

}