using AiTrainer.Web.Common;
using AiTrainer.Web.Common.Configuration;
using AiTrainer.Web.Common.Extensions;
using AiTrainer.Web.CoreClient.Clients.Abstract;
using AiTrainer.Web.CoreClient.Extensions;
using AiTrainer.Web.CoreClient.Models.Request;
using AiTrainer.Web.CoreClient.Models.Response;
using BT.Common.Http.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AiTrainer.Web.CoreClient.Clients.Concrete;

internal class CoreClientCreateFaissStore
    : ICoreClient<CoreCreateFaissStoreInput, CoreFaissStoreResponse>
{
    private readonly ILogger<CoreClientCreateFaissStore> _logger;
    private readonly AiTrainerCoreConfiguration _aiTrainerCoreConfiguration;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly HttpClient _httpClient;

    public CoreClientCreateFaissStore(
        ILogger<CoreClientCreateFaissStore> logger,
        HttpClient httpClient,
        IOptions<AiTrainerCoreConfiguration> aiTrainerCoreConfiguration,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _logger = logger;
        _aiTrainerCoreConfiguration = aiTrainerCoreConfiguration.Value;
        _httpContextAccessor = httpContextAccessor;
        _httpClient = httpClient;
    }

    public async Task<CoreFaissStoreResponse?> TryInvokeAsync(
        CoreCreateFaissStoreInput param,
        CancellationToken cancellation = default
    )
    {
        try
        {
            var correlationId = _httpContextAccessor.HttpContext?.GetCorrelationId();

            var response = await _aiTrainerCoreConfiguration.BaseEndpoint
                .AppendPathSegment("api")
                .AppendPathSegment("faissrouter")
                .AppendPathSegment("createstore")
                .WithCoreApiKeyHeader(_aiTrainerCoreConfiguration.ApiKey)
                .WithCorrelationIdHeader(correlationId?.ToString())
                .WithApplicationJson(param, ApiConstants.DefaultCamelCaseSerializerOptions)
                .PostJsonAsync<CoreResponse<CoreFaissStoreResponse>>(_httpClient,
                    ApiConstants.DefaultCamelCaseSerializerOptions, cancellation);

            return response?.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Exception occured in {OpName} with message {Message}",
                nameof(CoreClientCreateFaissStore),
                ex.Message
            );

            return null;
        }
    }
}
