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

internal class CoreClientFormattedChatQuery: ICoreClient<CoreFormattedChatQueryInput, CoreFormattedChatQueryResponse>
{
    private readonly ILogger<CoreClientFormattedChatQuery> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISerializer _serializer;

    public CoreClientFormattedChatQuery(
        ILogger<CoreClientFormattedChatQuery> logger,
        IOptionsSnapshot<AiTrainerCoreConfiguration> aiTrainerCoreConfig,
        IHttpContextAccessor httpContextAccessor,
        ISerializer serializer
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfig.Value;
        _httpContextAccessor = httpContextAccessor;
        _serializer = serializer;
    }

    public async Task<CoreFormattedChatQueryResponse?> TryInvokeAsync(CoreFormattedChatQueryInput request,
        CancellationToken cancellationToken = default)
    {
        var response = await _aiTrainerCoreConfiguration.BaseEndpoint
            .AppendPathSegment("api")
            .AppendPathSegment("openairouter")
            .AppendPathSegment("formattedchatquery")
            .WithAiTrainerCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
            .WithCorrelationIdHeader(_httpContextAccessor.HttpContext.GetCorrelationId())
            .WithSerializer(_serializer)
            .PostJsonAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken)
            .ReceiveJsonAsync<CoreResponse<CoreFormattedChatQueryResponse>>(_aiTrainerCoreConfiguration, cancellationToken)
            .CoreClientExceptionHandling(_logger, nameof(CoreClientFormattedChatQuery));
        
        return response?.Data;
    }
}