using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Models.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientHealth: BaseCoreClient<CoreClientHealthResponse>
{
    private const string _endPoint = "api/healthrouter";
    public CoreClientHealth(
        HttpClient httpClient,
        ILogger<CoreClientHealth> logger,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfig
    )
        : base(httpClient, aiTrainerCoreConfig, logger) { }

    protected override HttpRequestMessage BuildMessage()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = _aiTrainerCoreConfiguration.BaseEndpoint.AppendPathToUrl(
                _endPoint
            ),
        };
        AddApiKeyHeader(request);
            
        return request;
    }

}