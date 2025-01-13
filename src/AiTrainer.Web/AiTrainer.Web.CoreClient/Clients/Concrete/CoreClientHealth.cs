using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using BT.Common.Polly.Models.Concrete;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientHealth: BaseCoreClient<CoreClientHealthResponse>
{
    private const string _endPoint = "api/healthrouter";
    public CoreClientHealth(
        HttpClient httpClient,
        ILogger<CoreClientHealth> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig
    )
        : base(httpClient, aiTrainerCoreConfig, logger) { }

    public override async Task<CoreClientHealthResponse?> TryInvokeAsync()
    {
        try
        {
            var result = await TimeAndExecuteRequest(() => _httpClient.SendAsync(BuildMessage(), new PollyRetrySettings
            {
                TotalAttempts = 1,
                TimeoutInSeconds = 3,
            }));
            
            result.EnsureSuccessStatusCodeAndThrowCoreClientException();

            var data = await result.Content.ReadFromJsonAsync<CoreResponse<CoreClientHealthResponse>>();

            var actualData = data.EnsureSuccessfulCoreResponseAndGetData();

            return actualData;
        }
        catch (Exception e)
        {
            LogCoreError(e);
            return null;
        }
        
    }

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