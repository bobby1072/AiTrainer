using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.Common.Models.Configuration;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.HttpClient.Extensions;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientUpdateFaissStore: ICoreClient<UpdateFaissStoreInput, FaissStoreResponse>
{
    private readonly ILogger<CoreClientUpdateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CoreClientUpdateFaissStore(
        ILogger<CoreClientUpdateFaissStore> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<FaissStoreResponse?> TryInvokeAsync(UpdateFaissStoreInput input, CancellationToken cancellation = default)
    {
        await using var indexFileStream = new MemoryStream(input.FileInput);
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("faissrouter")
            .AppendPathSegment("updatestore")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .PostMultipartAsync(x =>
            {
                x.AddJson("metadata", input);
                x.AddFile("file", indexFileStream, "docStore.index");
            }, HttpCompletionOption.ResponseContentRead, cancellation)
            .ReceiveJsonAsync<CoreResponse<FaissStoreResponse>>(_aiTrainerCoreConfiguration, cancellation)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientUpdateFaissStore));
        
        
        
        return response?.Data;
    }
}