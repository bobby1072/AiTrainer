using System.Text.Json;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientCreateFaissStore : ICoreClient<CreateFaissStoreInput, CreateFaissStoreResponse>
{
    private readonly ILogger<CoreClientCreateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;

    public CoreClientCreateFaissStore(
        ILogger<CoreClientCreateFaissStore> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfiguration
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
    }

    public async Task<CreateFaissStoreResponse?> TryInvokeAsync(CreateFaissStoreInput param)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("faissrouter")
            .AppendPathSegment("createstore")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .PostJsonAsync(param)
            .ReceiveJsonAsync<CoreResponse<CreateFaissStoreResponse>>(_aiTrainerCoreConfiguration)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientCreateFaissStore));
        
        return response?.Data;
    }
}