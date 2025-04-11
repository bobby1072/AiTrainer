using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Extensions;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientCreateFaissStore : ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientCreateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISerializer _serialiser;

    public CoreClientCreateFaissStore(
        ILogger<CoreClientCreateFaissStore> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfiguration,
        IHttpContextAccessor httpContextAccessor,
        ISerializer serialiser

    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _serialiser = serialiser;
    }

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(CoreCreateFaissStoreInput param, CancellationToken cancellation = default)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("faissrouter")
            .AppendPathSegment("createstore")
            .WithAiTrainerCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .WithSerializer(_serialiser)
            .PostJsonAsync(param, HttpCompletionOption.ResponseHeadersRead, cancellation)
            .ReceiveJsonAsync<CoreResponse<CoreFaissStoreResponse>>(_aiTrainerCoreConfiguration, cancellation)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientCreateFaissStore));
        
        return response?.Data;
    }
}