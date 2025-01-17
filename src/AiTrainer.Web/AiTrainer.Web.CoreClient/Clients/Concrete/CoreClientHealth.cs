using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using BT.Common.Helpers.Extensions;
using BT.Common.Polly.Models.Concrete;
using Flurl;
using Flurl.Http;

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
        try
        {
            var response = await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("healthrouter")
                .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .GetJsonAsync<CoreClientHealthResponse>(_aiTrainerCoreConfiguration);
            
                
            return response;
        }
        catch (FlurlHttpException ex)
        {
            _logger.LogError(ex, "{NameOfOp} request failed with status code {StatusCode}",
                nameof(CoreClientHealth),
                ex.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{NameOfOp} request failed",
                nameof(CoreClientHealth));
            return null;
        }
    }

}