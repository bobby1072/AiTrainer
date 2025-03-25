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

public class CoreClientSimilaritySearch: ICoreClient<CoreSimilaritySearchInput, SimilaritySearchCoreResponse>
{
    private readonly ILogger<CoreClientSimilaritySearch> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISerializer _serialiser;

    public CoreClientSimilaritySearch(
        ILogger<CoreClientSimilaritySearch> logger,
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

    public async Task<SimilaritySearchCoreResponse?> TryInvokeAsync(CoreSimilaritySearchInput input, CancellationToken cancellation = default)
    {
        await using var indexFileStream = new MemoryStream(input.FileInput);
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("faissrouter")
            .AppendPathSegment("similaritysearch")
            .WithAiTrainerCoreKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .WithSerializer(_serialiser)
            .PostMultipartAsync(x =>
            {
                x.AddJson("metadata", input);
                x.AddFile("file", indexFileStream, "docStore.index");
            }, HttpCompletionOption.ResponseContentRead, cancellation)
            .ReceiveJsonAsync<CoreResponse<SimilaritySearchCoreResponse>>(_aiTrainerCoreConfiguration, cancellation)
            .CoreClientExceptionHandling(_logger, nameof(FaissStoreResponse));
        
        
        return response?.Data;
    } 
}
